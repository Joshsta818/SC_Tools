using Microsoft.VisualBasic;
using Microsoft.CSharp;
using System;
using System.Management;

namespace SC_Tools
{
    public class CallWMIMethod
    {
        public static void NRTCRestore(string restoreName)
        {

            dynamic restPoint = Interaction.GetObject("winmgmts:\\\\.\\root\\default:Systemrestore");
            if (restPoint != null)
            {
                if (restPoint.CreateRestorePoint(restoreName, 0, 100) == 0)
                {
                    Console.WriteLine("Restore Point created successfully");
                }
                else
                {
                    Console.WriteLine("Could not create restore point!");
                }
            }
        }
    }
}