using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HEN_UNGEN
{
    class Hen_ungen
    {
        static long swap64(FileStream src)
        {
            byte[] buffer = new byte[sizeof(long)];
            src.Read(buffer, 0, buffer.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        static void dump(FileStream src, long size, string filename)
        {
            using (FileStream target = File.Create(filename))
            {
                byte[] buffer = new byte[size];

                if (src.Read(buffer, 0, buffer.Length) != size)
                    throw new InvalidDataException(filename + " is incomplete");

                target.Write(buffer, 0, buffer.Length);
            }
        }

        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("\nPS3HEN.BIN Unpacker\nUsage:PS3HEN.BIN\n");
                return -1;
            }

            try
            {
                using (FileStream sp = File.Open(args[0], FileMode.Open, FileAccess.Read))
                {
                    sp.Position = 0x7fff8;
                    long stage2_size = swap64(sp);
                    sp.Position = 0x80000;
                    dump(sp, stage2_size, "stage2.bin");

                    sp.Position = 0x1008b8;
                    long stage0_size = swap64(sp);
                    sp.Position = 0x102000;
                    dump(sp, stage0_size, "stage0.bin");

                    sp.Position = 0x70010;// read sprx size directly from SCE header (inside of the SPRX)
                    long HENplugin_size = swap64(sp) + swap64(sp);// sprx_size = header_length + data_length;
                    sp.Position = 0x70000;
                    dump(sp, HENplugin_size, "HENplugin.sprx");
                }

                Console.WriteLine("DONE!\n");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("error creating file(s)! {0}\n", e.Message);
                return -1;
            }
        }
    }
}
