using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace gouniversalDesktop
{
    class runprocess
    {
        private Thread exeThread;
        private Process exeProcess;
        private readonly object processLock = new object();
        private bool hasStarted;
        private bool hasExited;
        private string exeFileName;

        public void Start(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                return;
            }

            lock (processLock)
            {
                hasStarted = false;
                hasExited = false;
                exeFileName = fileName;
            }

            exeThread = new Thread(gouThread);
            exeThread.Start();

            var spin = new SpinWait();

            while (true)
            {
                lock (processLock)
                {
                    if (hasStarted)
                    {
                        return;
                    }
                }

                spin.SpinOnce();
            }
        }

        public void Stop()
        {
            bool exited;

            lock (processLock)
            {
                if (hasStarted == false)
                {
                    return;
                }

                exited = hasExited;
            }

            if (exited == false)
            {
                exeProcess.Kill();
                exeProcess.WaitForExit();
                exeProcess.Close();

                var spin = new SpinWait();

                while (true)
                {
                    lock (processLock)
                    {
                        if (hasExited)
                        {
                            exeThread.Abort();
                            exeThread.Join();
                            return;
                        }
                    }

                    spin.SpinOnce();
                }
            }

            exeThread.Abort();
            exeThread.Join();
        }

        public bool IsRunning()
        {
            lock (processLock)
            {
                return !hasExited;
            }
        }


        private void gouThread()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            lock (processLock)
            {
                startInfo.FileName = exeFileName;
                hasStarted = true;
            }

            try
            {
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            lock (processLock)
            {
                hasExited = true;
            }
        }
    }
}
