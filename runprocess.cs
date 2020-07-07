using System.Diagnostics;
using System.Threading;

namespace gouniversalDesktop
{
    class runprocess
    {
        private Thread exeThread;
        private Process exeProcess;
        private readonly object processLock = new object();
        private bool hasExited;
        private string exeFileName;

        public void Start(string fileName)
        {
            lock (processLock)
            {
                hasExited = true;
                exeFileName = fileName;
            }

            exeThread = new Thread(gouThread);
            exeThread.Start();

            var spin = new SpinWait();

            while (true)
            {
                lock (processLock)
                {
                    if (hasExited == false)
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
                exited = hasExited;
            }

            if (exited == false) {

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

            lock (processLock)
            {
                startInfo.FileName = exeFileName;
            }

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                exeProcess = Process.Start(startInfo);

                lock (processLock)
                {
                    hasExited = false;
                }

                exeProcess.WaitForExit();
            }
            catch
            {
                // Log error.
            }

            lock (processLock)
            {
                hasExited = true;
            }
        }
    }
}
