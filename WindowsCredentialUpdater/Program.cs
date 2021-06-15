using AdysTech.CredentialManager;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsCredentialUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine("ALgomedi Credential Manager Password Updater");
                Console.WriteLine("------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                var targetPath = Path.Combine(Environment.CurrentDirectory, "Targets.txt");
                if (!File.Exists(targetPath))
                {
                    using (var fs = File.Create(targetPath))
                    {
                        var credentialTargetsStringBuilder = new StringBuilder();
                        //It could be different for each pc
                        credentialTargetsStringBuilder.AppendLine("Credential Target");


                        Byte[] title = new UTF8Encoding(true).GetBytes(credentialTargetsStringBuilder.ToString());
                        fs.Write(title, 0, title.Length);
                        fs.Flush();
                    }
                }
                var targets = File.ReadAllLines(targetPath);
                var allCredentials = CredentialManager.EnumerateICredentials();
                var relatedWithAlgomedi = allCredentials.Where(x => targets.Contains(x.TargetName));
                if (!relatedWithAlgomedi.Any())
                {
                    Console.WriteLine("There is a no credentials found. Please check the Targets.txt file");
                    return;
                }

                Console.WriteLine("Please tell me new Algomedi password");
                var newPassword = Console.ReadLine();
                foreach (var item in relatedWithAlgomedi)
                {
                    CredentialManager.SaveCredentials(
                        item.TargetName,
                        new System.Net.NetworkCredential
                        {
                            UserName = item.UserName,
                            Password = newPassword
                        },
                        item.Type);
                    Console.WriteLine($"Updated Credential Target {item.TargetName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Opps Something went wrong. {Environment.NewLine}{Environment.NewLine} {JsonConvert.SerializeObject(ex)}");
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine($"Press any key to close");
                Console.ReadKey();
            }
        }
    }
}
