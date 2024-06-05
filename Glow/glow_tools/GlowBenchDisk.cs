﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using static Glow.GlowModules;

namespace Glow.glow_tools{
    public partial class GlowBenchDisk : Form{
        // ======================================================================================================
        // GLOBAL LANGS PATH
        TSGetLangs g_lang = new TSGetLangs(Glow.lang_path);
        // VARIABLES
        private Thread benchmarkThread;
        private bool isBenchmarking = false;
        string benchmarkFilePath;
        int global_buffer;
        //
        List<string> benchmarkDiskList = new List<string>();
        List<double> benchmarkDiskListFreeSpace = new List<double>();
        List<string> benchmarkDiskListType = new List<string>();
        List<long> testSizes = new List<long>();
        public GlowBenchDisk(){ InitializeComponent(); CheckForIllegalCrossThreadCalls = false; }
        // THEME MODE
        public void bench_disk_theme_settings(){
            try{
                if (Glow.theme == 1){
                    try { if (DwmSetWindowAttribute(Handle, 20, new[]{ 1 }, 4) != 1){ DwmSetWindowAttribute(Handle, 20, new[]{ 0 }, 4); } }catch (Exception){ }
                }else if (Glow.theme == 0 || Glow.theme == 2){
                    try { if (DwmSetWindowAttribute(Handle, 19, new[]{ 1 }, 4) != 0){ DwmSetWindowAttribute(Handle, 20, new[]{ 1 }, 4); } }catch (Exception){ }
                }
                BackColor = Glow.ui_colors[5];
                //
                Bench_P1.BackColor = Glow.ui_colors[6];
                Bench_P2.BackColor = Glow.ui_colors[6];
                Bench_P3.BackColor = Glow.ui_colors[6];
                Bench_P4.BackColor = Glow.ui_colors[6];
                Bench_P5.BackColor = Glow.ui_colors[6];
                Bench_P6.BackColor = Glow.ui_colors[6];
                Bench_P7.BackColor = Glow.ui_colors[6];
                //
                Bench_Label_Disk.ForeColor = Glow.ui_colors[7];
                Bench_Disk.BackColor = Glow.ui_colors[10];
                Bench_Disk.ForeColor = Glow.ui_colors[8];
                //
                Bench_Label_Size.ForeColor = Glow.ui_colors[7];
                Bench_Size.BackColor = Glow.ui_colors[10];
                Bench_Size.ForeColor = Glow.ui_colors[8];
                Bench_SizeCustom.BackColor = Glow.ui_colors[11];
                Bench_SizeCustom.ForeColor = Glow.ui_colors[12];
                //
                Bench_Label_Buffer.ForeColor = Glow.ui_colors[7];
                Bench_Buffer.BackColor = Glow.ui_colors[10];
                Bench_Buffer.ForeColor = Glow.ui_colors[8];
                //
                Bench_Label_LTimeWrite.ForeColor = Glow.ui_colors[7];
                Bench_Label_LTimeWriteResult.ForeColor = Glow.ui_colors[8];
                Bench_Label_RSpeedWrite.ForeColor = Glow.ui_colors[7];
                Bench_Label_RSpeedWriteResult.ForeColor = Glow.ui_colors[8];
                Bench_Label_LTimeRead.ForeColor = Glow.ui_colors[7];
                Bench_Label_LTimeReadResult.ForeColor = Glow.ui_colors[8];
                Bench_Label_RSpeedRead.ForeColor = Glow.ui_colors[7];
                Bench_Label_RSpeedReadResult.ForeColor = Glow.ui_colors[8];
                //
                Bench_Start.BackColor = Glow.ui_colors[8];
                Bench_Start.ForeColor = Glow.ui_colors[19];
                Bench_Stop.BackColor = Glow.ui_colors[8];
                Bench_Stop.ForeColor = Glow.ui_colors[19];
                //
                Text = string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_title").Trim())), Application.ProductName);
                Bench_Label_Disk.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_select_disk").Trim()));
                Bench_Label_Size.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_test_size").Trim()));
                Bench_Label_Buffer.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_test_buffer_size").Trim()));
                //
                Bench_Label_LTimeWrite.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_time_write").Trim()));
                Bench_Label_LTimeRead.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_time_read").Trim()));
                Bench_Label_RSpeedWrite.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_speed_write").Trim()));
                Bench_Label_RSpeedRead.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_speed_read").Trim()));
                //
                Bench_Start.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start").Trim()));
                Bench_Stop.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_stop").Trim()));
                //
                Bench_Size.Items[6] = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_test_size_custom").Trim()));
            }catch (Exception){ }
        }
        // LOAD
        private void GlowBenchDisk_Load(object sender, EventArgs e){
            RefreshDriveList();
            //
            Bench_Size.Items.Add("10 GB");
            Bench_Size.Items.Add("15 GB");
            Bench_Size.Items.Add("20 GB");
            Bench_Size.Items.Add("25 GB");
            Bench_Size.Items.Add("32 GB");
            Bench_Size.Items.Add("64 GB");
            Bench_Size.Items.Add(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_test_size_custom").Trim())));
            testSizes.Add(10);
            testSizes.Add(15);
            testSizes.Add(20);
            testSizes.Add(25);
            testSizes.Add(32);
            testSizes.Add(64);
            Bench_Size.SelectedIndex = 0;
            //
            Bench_Buffer.Items.Add("64 KB");
            Bench_Buffer.Items.Add("128 KB");
            Bench_Buffer.Items.Add("256 KB");
            Bench_Buffer.Items.Add("512 KB");
            Bench_Buffer.Items.Add("1024 KB");
            Bench_Buffer.Items.Add("4096 KB");
            Bench_Buffer.SelectedIndex = 4;
            //
            Bench_Label_LTimeWriteResult.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_test_await").Trim()));
            Bench_Label_LTimeReadResult.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_test_await").Trim()));
            Bench_Label_RSpeedWriteResult.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_test_await").Trim()));
            Bench_Label_RSpeedReadResult.Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_test_await").Trim()));
            //
            bench_disk_theme_settings();
        }
        // DISK LIST
        private void RefreshDriveList(){
            Bench_Disk.Items.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives){
                string driveInfo = "";
                if (drive.VolumeLabel.ToLower().Trim() == "" || drive.VolumeLabel == string.Empty){
                    driveInfo = $"{Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_select_local_disk").Trim()))} ({drive.Name.Replace("\\", string.Empty)}) - {TS_FormatSize(drive.TotalSize)}";
                }else{ 
                    driveInfo = $"{drive.VolumeLabel} ({drive.Name.Replace("\\", string.Empty)}) - {TS_FormatSize(drive.TotalSize)}";
                }
                Bench_Disk.Items.Add(driveInfo);
                benchmarkDiskList.Add(drive.Name);
                benchmarkDiskListFreeSpace.Add(drive.TotalFreeSpace / 1024 / 1024 / 1024);
                benchmarkDiskListType.Add(drive.DriveType.ToString().ToLower().Trim());
            }
            Bench_Disk.SelectedIndex = 0;
        }
     
        // START BTN
        private void Bench_Start_Click(object sender, EventArgs e){
            if (Bench_Size.SelectedIndex == 6){
                if (Bench_SizeCustom.Text.Trim() != "" || Bench_SizeCustom.Text.Trim() != string.Empty){
                    if (Convert.ToDouble(Bench_SizeCustom.Text.Trim()) >= 10){
                        if (benchmarkDiskListFreeSpace[Bench_Disk.SelectedIndex] > Convert.ToInt32(Bench_SizeCustom.Text.Trim())){
                            check_info_user_warning(Bench_Disk.SelectedIndex);
                        }
                        else{
                            MessageBox.Show(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_low_space").Trim())), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }else{
                        MessageBox.Show(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_space_10").Trim())), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }else{
                    return;
                }
            }else{
                if (benchmarkDiskListFreeSpace[Bench_Disk.SelectedIndex] > testSizes[Bench_Disk.SelectedIndex]){
                    check_info_user_warning(Bench_Disk.SelectedIndex);
                }else{
                    MessageBox.Show(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_low_space").Trim())), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        // CHECK DISK USER INFO
        private void check_info_user_warning(int info_mode){
            try{
                DialogResult sucess_warning;
                string mode = benchmarkDiskListType[info_mode];
                switch (mode){
                    case "cdrom":
                        // CD ROM
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_cdrom").Trim())), "\n\n", "\n\n", "\n", "\n\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (sucess_warning == DialogResult.Yes){
                            start_engine();
                        }
                        break;
                    case "fixed":
                        // FIXED
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_fixed").Trim())), "\n\n", "\n\n", "\n", "\n\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (sucess_warning == DialogResult.Yes){
                            start_engine();
                        }
                        break;
                    case "network":
                        // NETWORK
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_network").Trim())), "\n\n", "\n\n", "\n", "\n\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (sucess_warning == DialogResult.Yes){
                            start_engine();
                        }
                        break;
                    case "norootdirectory":
                        // NO ROOT DIRECTORY
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_nrd").Trim())), "\n\n", "\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case "ram":
                        // RAM
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_ram").Trim())), "\n\n", "\n"),  Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case "removable":
                        // REMOVABLE
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_removable").Trim())), "\n\n", "\n\n", "\n", "\n\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (sucess_warning == DialogResult.Yes){
                            start_engine();
                        }
                        break;
                    default:
                        // UNKNOW
                        sucess_warning = MessageBox.Show(string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_disk_unknown").Trim())), "\n\n"), Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_start_engine_disk").Trim())) + " " + Bench_Disk.SelectedItem.ToString().Trim(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }catch (Exception){ }
        }
        // START ENGINE
        private void start_engine(){
            // SELECT DRIVE
            string selectedDrive = benchmarkDiskList[Bench_Disk.SelectedIndex];
            // WARNING MESSAGE
            try{
                isBenchmarking = true;
                benchmarkThread = new Thread(() => RunBenchmark(selectedDrive));
                benchmarkThread.Start();
                Bench_Start.Enabled = false;
                Bench_Stop.Enabled = true;
                Bench_Disk.Enabled = false;
                Bench_Size.Enabled = false;
                Bench_SizeCustom.Enabled = false;
                Bench_Buffer.Enabled = false;
                Bench_SizeCustom.Enabled = false;
            }catch (Exception){ }
        }
        // GB TO BYTE
        static long GigabytesToBytes(double gigabytes){
            return (long)(gigabytes * 1024 * 1024 * 1024);
        }
        // KB TO BYTE
        static byte[] KilobytesToBytes(double kilobytes){
            return new byte[(long)(kilobytes * 1024)];
        }
        // UPDATE PROGRESS
        private void UpdateProgress(double progress){
            Text = string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_title").Trim())), Application.ProductName) + " - " + progress.ToString("0.00") + "%";
        }
        // DISK BENCHMARK
        private void RunBenchmark(string selectedDrive){
            // FILE PATH
            benchmarkFilePath = Path.Combine(selectedDrive, "GlowBenchDiskTestFile_" + new Random().Next(1000, 9999) + ".glow");
            // BENCH SIZE
            long fileSizeInBytes = 0;
            if (Bench_Size.SelectedIndex == 0){
                fileSizeInBytes = GigabytesToBytes(10);  // 10 GB
            }else if (Bench_Size.SelectedIndex == 1){
                fileSizeInBytes = GigabytesToBytes(15);  // 15 GB
            }else if (Bench_Size.SelectedIndex == 2){
                fileSizeInBytes = GigabytesToBytes(20); // 20 GB
            }else if (Bench_Size.SelectedIndex == 3){
                fileSizeInBytes = GigabytesToBytes(25); // 25 GB
            }else if (Bench_Size.SelectedIndex == 4){
                fileSizeInBytes = GigabytesToBytes(32); // 32 GB
            }else if (Bench_Size.SelectedIndex == 5){
                fileSizeInBytes = GigabytesToBytes(64); // 64 GB
            }else if (Bench_Size.SelectedIndex == 6){
                int _cs = Convert.ToInt32(Bench_SizeCustom.Text.Trim());
                fileSizeInBytes = GigabytesToBytes(_cs); // Custom Size
            }
            // BENCH BUFFER POOL
            byte[] buffer = KilobytesToBytes(0);
            if (Bench_Buffer.SelectedIndex == 0){
                global_buffer = 64; // 64 KB
            }else if (Bench_Buffer.SelectedIndex == 1){
                global_buffer = 128; // 128 KB
            }else if (Bench_Buffer.SelectedIndex == 2){
                global_buffer = 256; // 256 KB
            }else if (Bench_Buffer.SelectedIndex == 3){
                global_buffer = 512; // 512 KB
            }else if (Bench_Buffer.SelectedIndex == 4){
                global_buffer = 1024; // 1024 KB
            }else if (Bench_Buffer.SelectedIndex == 5){
                global_buffer = 4096; // 4096 KB
            }
            buffer = KilobytesToBytes(global_buffer);
            // BENCHMARK SETTINGS
            Stopwatch writeStopwatch = Stopwatch.StartNew();
            using (FileStream fs = new FileStream(benchmarkFilePath, FileMode.Create, FileAccess.Write)){
                long bytesWritten = 0;
                while (bytesWritten < fileSizeInBytes && isBenchmarking){
                    int bufferSize = (int)Math.Min(buffer.Length, fileSizeInBytes - bytesWritten);
                    fs.Write(buffer, 0, bufferSize);
                    bytesWritten += bufferSize;
                    double progress = (double)bytesWritten / fileSizeInBytes * 100;
                    UpdateProgress(progress);
                }
            }
            writeStopwatch.Stop();
            // BENCHMARK MODE STATUS
            if (isBenchmarking){
                // BENCHMARK STREAM
                Stopwatch readStopwatch = Stopwatch.StartNew();
                using (FileStream fs = new FileStream(benchmarkFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, global_buffer)){
                    long totalBytesRead = 0;
                    long fileSize = fs.Length;
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0 && isBenchmarking){
                        totalBytesRead += bytesRead;
                        double progress = (double)totalBytesRead / fileSize * 100;
                        UpdateProgress(progress);
                    }
                }
                readStopwatch.Stop();
                // CONVERT TO DOUBLE
                double writeSpeed = fileSizeInBytes / writeStopwatch.Elapsed.TotalSeconds;
                double readSpeed = fileSizeInBytes / readStopwatch.Elapsed.TotalSeconds;
                // ACTION TEST RESULT
                Invoke(new Action(() => {
                    Bench_Label_LTimeWriteResult.Text = ($"{string.Format("{0:0.00}", writeStopwatch.Elapsed.TotalSeconds)} {Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_result_second").Trim()))}");
                    Bench_Label_LTimeReadResult.Text = ($"{string.Format("{0:0.00}", readStopwatch.Elapsed.TotalSeconds)} {Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_result_second").Trim()))}");
                    Bench_Label_RSpeedWriteResult.Text = ($"{string.Format("{0:0.00}", writeSpeed / 1024 / 1024)} MB/s");
                    Bench_Label_RSpeedReadResult.Text = ($"{string.Format("{0:0.00}", readSpeed / 1024 / 1024)} MB/s");
                }));
                // TEST AFTER DELETE
                if (File.Exists(benchmarkFilePath)){
                    File.Delete(benchmarkFilePath);
                    Invoke(new Action(() => {
                        Text = string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_title").Trim())), Application.ProductName);
                        MessageBox.Show(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_result_success").Trim())), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                //
                isBenchmarking = false;
                Bench_Start.Enabled = true;
                Bench_Stop.Enabled = false;
                Bench_Disk.Enabled = true;
                Bench_Size.Enabled = true;
                Bench_SizeCustom.Enabled = true;
                Bench_Buffer.Enabled = true;
                Bench_SizeCustom.Enabled = true;
            }
        }
        // STOP BENCHMARK
        private void Bench_Stop_Click(object sender, EventArgs e){
            stop_engine();
        }
        private void stop_engine(){
            isBenchmarking = false;
            if (benchmarkThread != null && benchmarkThread.IsAlive){
                benchmarkThread.Join();
            }
            Bench_Start.Enabled = true;
            Bench_Stop.Enabled = false;
            Bench_Disk.Enabled = true;
            Bench_Size.Enabled = true;
            Bench_SizeCustom.Enabled = true;
            Bench_Buffer.Enabled = true;
            Bench_SizeCustom.Enabled = true;
            if (File.Exists(benchmarkFilePath)){
                File.Delete(benchmarkFilePath);
                Text = string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_title").Trim())), Application.ProductName);
                MessageBox.Show(Encoding.UTF8.GetString(Encoding.Default.GetBytes(g_lang.TSReadLangs("BenchDisk", "bd_title").Trim())), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // CUSTOM SIZE CHANGE
        private void Bench_Size_SelectedIndexChanged(object sender, EventArgs e){
            if (Bench_Size.SelectedIndex == 6){
                Bench_SizeCustom.Visible = true;
            }else{
                Bench_SizeCustom.Visible = false;
            }
        }
        // NUMERIC INPUT
        private void Bench_SizeCustom_KeyPress(object sender, KeyPressEventArgs e){
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)){
                e.Handled = true;
            }
        }
        // EXIT STOP ENGINE
        private void GlowBenchDisk_FormClosing(object sender, FormClosingEventArgs e){
            stop_engine();
        }
    }
}