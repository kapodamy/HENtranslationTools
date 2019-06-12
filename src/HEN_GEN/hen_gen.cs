using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HEN_GEN
{
    class Hen_gen
    {
        static byte[] swap64(long data)
        {
            byte[] ret = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(ret);
            return ret;
        }

        static void dump(FileStream target, FileStream src)
        {
            byte[] buffer = new byte[src.Length];

            if (src.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidDataException(src.Name + " is incomplete");

            target.Write(buffer, 0, buffer.Length);
            src.Close();
        }


        static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage:stackframe.bin stage2 stage0 sprx\n");
                return -1;
            }

            FileStream sp = null;
            FileStream stage2 = null;
            FileStream stage0 = null;
            FileStream sprx = null;

            try
            {
                sp = File.Open(args[0], FileMode.Open, FileAccess.ReadWrite);
                stage2 = File.Open(args[1], FileMode.Open, FileAccess.ReadWrite);
                stage0 = File.Open(args[2], FileMode.Open, FileAccess.ReadWrite);
                sprx = File.Open(args[3], FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("error opening file(s)! {0}\n", e.Message);
                return -1;
            }

            uint truncate_len = 0x110000;

            byte[] size_stage2 = swap64(stage2.Length);
            sp.Position = 0x7fff8;
            sp.Write(size_stage2, 0, size_stage2.Length);
            //	truncate_len+=stage2.Length;
            dump(sp, stage2);

            byte[] size_stage0 = swap64(stage0.Length);
            sp.Position = 0x102000;
            dump(sp, stage0);
            sp.Position = 0x1008b8;
            sp.Write(size_stage0, 0, size_stage0.Length);
            
            sp.Position = 0x70000;
            dump(sp, sprx);

            sp.SetLength(truncate_len);
            sp.Close();
            Console.WriteLine("DONE!\n");
            return 0;
        }
    }
}
