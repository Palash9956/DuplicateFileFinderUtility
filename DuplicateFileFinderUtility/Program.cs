using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace DuplicateFileFinderUtility
{
    class DuplicateFileFinderUtility
    {
        public static string sourceDirectory = null;
        public static string str = "";
        public static int num = 0;
        public static int totalfiles = 0;


        static void Main(string[] args)
        {
            DuplicateFileFinderUtility ob = new DuplicateFileFinderUtility();
            ob.ShowWarningAndThenContinue();
            ob.DuplicateFileSearch();
            //ob.GetPath();
            Console.Read();
        }

        private static void ProgressDisplay()
        {
            try
            {


                DuplicateFileFinderUtility ob = new DuplicateFileFinderUtility();
                int index = 0, k = 0;
                Console.Clear();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                List<string> listfiles = new List<string>();
                List<string> hashlist = new List<string>();
                HashSet<string> seen = new HashSet<string>();
                List<string> duplicateFree = new List<string>();
                List<int> deletedIndices = new List<int>();


                string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                totalfiles = files.Length;
                Console.WriteLine("{0} File(s) Found.", totalfiles);
                foreach (string n in files)
                {
                    listfiles.Add(n);
                }

                Console.WriteLine("Searching For Duplicate Files...");
                num = 1;
                foreach (string fileslist in listfiles)
                {
                    ob.CurrentFileName(fileslist);


                    num++;
                    Console.SetCursorPosition(0, 4);
                    Console.Write("{0}/{1} Files Scanned{2}", num, totalfiles, str);
                    //Console.Write("{0}/{1} Files Scanned{2}", num++, totalfiles, ob.dot());
                    Console.SetCursorPosition(0, 5);
                    Console.Write("{0}% Completed", Convert.ToInt32(((double)num / (double)totalfiles) * (double)100));
                    hashlist.Add(GetSHA512HashChecksum(fileslist));
                }

                foreach (var s in hashlist)
                {
                    if (seen.Add(s))
                    {
                        duplicateFree.Add(s);
                    }
                    else
                    {
                        deletedIndices.Add(index);
                    }
                    ++index;
                }

                stopwatch.Stop();
                Console.WriteLine();

                if (deletedIndices.Count == 0)
                {
                    Console.WriteLine("Congrats, There Are NO Duplicate Files");
                }
                else
                {
                    Console.WriteLine("Found {0} Duplicate Files In {1}", deletedIndices.Count, ob.GetTime(stopwatch.ElapsedMilliseconds));
                    Console.WriteLine("What Do You Want To Do?");

                    while (deletedIndices.Count != 0)
                    {
                        Console.WriteLine("Press \"N\" To Delete Files One By One Or Press \"A\" To Delete All The Files At Once Or Press \"X\" To Exit");
                        ConsoleKeyInfo info = Console.ReadKey();

                        if (info.KeyChar == 'N' || info.KeyChar == 'n')
                        {
                            Console.WriteLine("\n{0} Was Deleted", Path.GetFileName(listfiles[deletedIndices[k]]));
                            File.Delete(listfiles[deletedIndices[k]]);
                            deletedIndices.RemoveAt(k);
                            k++;

                        }

                        else if (info.KeyChar == 'A' || info.KeyChar == 'a')
                        {
                            Console.Clear();
                            Console.WriteLine("There Are Total {0} File(s) To Be Deleted", deletedIndices.Count);
                            Console.WriteLine("Deleting Of Files Started..");
                            num = 1;
                            totalfiles = deletedIndices.Count;
                            foreach (int str in deletedIndices)
                            {
                                ob.CurrentFileName(listfiles[str]);
                                Console.SetCursorPosition(0, 4);
                                //Console.Write("{0}/{1} Files Deleted{2}", num++, totalfiles, ob.dot());
                                Console.SetCursorPosition(0, 5);
                                Console.Write("{0}% Completed", ob.percent(num, deletedIndices.Count));
                                File.Delete(listfiles[str]);
                            }
                            Console.WriteLine("\nAll The Duplicate Files Were Deleted");
                            deletedIndices.Clear();
                        }

                        else if (info.KeyChar == 'X' || info.KeyChar == 'x')
                        {
                            return;
                        }

                        else
                        {
                            Console.WriteLine("\nWrong Key Pressed..Please Try Again");
                        }
                    }
                }
                Console.WriteLine("Press Any Key To Continue..");

            }


            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Any Key To Exit And Try Again");
            }


        }

        public void ShowWarningAndThenContinue()
        {
            try
            {
                Console.Title = "Duplicate File Finder Utility v2.2.1.3";
                Console.WriteLine("***Duplicate File Finder Utility v2.2***");
                Console.WriteLine("THIS UTILITY IS 100% PERFECTLY WORKING CORRECTLY");
                Console.WriteLine("THIS UTILITY WILL DELETE ONLY DUPLICATE FILES FROM THE FOLDER & ITS SUB-FOLDER");
                Console.WriteLine("I WILL NOT BE RESPONSIBLE FOR AN IMPORTANT FILE YOU MISS FROM THIS UTILITY");
                Console.WriteLine("IF ANY BUGS FOUND THEN PLEASE REPORT IT TO \"PALASHSACHAN@GMAIL.COM\"");
                Console.WriteLine("SO, PLEASE PROCEED AT YOUR OWN RISK ONLY");
                Console.WriteLine("Press Any Key To Proceed Or Press \"X\" To Exit.");

                ConsoleKeyInfo consolekeyinfo = Console.ReadKey();
                if (consolekeyinfo.KeyChar == 'X' || consolekeyinfo.KeyChar == 'x')
                {
                    Environment.Exit(1);
                }
                else
                {
                    Console.Clear();
                    GetPath();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Any Key To Continue..");
            }
        }

        public void GetPath()
        {
            try
            {
                Console.WriteLine("Please Enter Full Directory Path");

                sourceDirectory = Console.ReadLine();
                DuplicateFileSearch();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Any Key To Continue..");
            }

        }

        public void DuplicateFileSearch()
        {

            var Thread1 = new Thread(dot);
            var Thread2 = new Thread(ProgressDisplay);
            Thread1.Start();
            Thread2.Start();
            //Thread1.Join();
            //Thread2.Join();


        }

        public string percent(int n, int count)
        {
            //double percent = (double)(n * 100) / count; // <-- Use cast
            return Math.Round(Convert.ToDecimal((n * 100) / count), 2).ToString("0.00"); // <-- Using cast
        }

        private static string GetSHA512HashChecksum(String name)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FileStream fs = new FileStream(name, FileMode.Open);
                BufferedStream bs = new BufferedStream(fs);
                {

                    //MD5CryptoServiceProvider sha512 = new MD5CryptoServiceProvider();
                    //SHA512Managed sha512 = new SHA512Managed();
                    SHA1CryptoServiceProvider sha512 = new SHA1CryptoServiceProvider();
                    //SHA256CryptoServiceProvider sha512 = new SHA256CryptoServiceProvider();
                    //SHA384CryptoServiceProvider sha512 = new SHA384CryptoServiceProvider();
                    //SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
                    {
                        byte[] hash = sha512.ComputeHash(bs);

                        foreach (byte b in hash)
                        {
                            sb.AppendFormat("{0:X2}", b);
                        }
                        sha512.Dispose();
                    }
                }
                bs.Close();
                fs.Close();
                return sb.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string GetTime(long TimeInMilliseconds)
        {
            if (TimeInMilliseconds < 1000)
            {
                return TimeInMilliseconds + " MilliSecond(s)";
            }
            else if (TimeInMilliseconds >= 1000 && TimeInMilliseconds < 60000)
            {
                return TimeInMilliseconds / 1000 + " Second(s)";
            }
            else if (TimeInMilliseconds >= 60000 && TimeInMilliseconds < 3600000)
            {
                return (TimeInMilliseconds / 1000) / 60 + " Minute(s)";
            }
            else if (TimeInMilliseconds >= 3600000)
            {
                return ((TimeInMilliseconds / 1000) / 60) / 60 + " Hour(s)";
            }
            else
            {
                return TimeInMilliseconds + " ms";
            }
        }

        public void CurrentFileName(string file)
        {
            Console.SetCursorPosition(0, 3);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.SetCursorPosition(0, 3);
            Console.Write("Current File Name :- {0}", Path.GetFileName(file));
        }

        public static void dot()
        {
            Console.CursorVisible = false;
            for (; ; )
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (i == 1)
                    {
                        str = ".";
                        Thread.Sleep(1000);
                    }

                    else if (i == 2)
                    {
                        str = "..";
                        Thread.Sleep(1000);
                    }

                    else if (i == 3)
                    {
                        str = "...";
                        Thread.Sleep(1000);
                    }

                    else
                    {
                        str = "   ";
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}