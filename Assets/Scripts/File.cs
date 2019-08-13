using System.IO;

namespace ProjectHighSchool {
    public static class File {
        public static byte[] ReadAllBytes(string path) {
            using (FileStream fsSource = System.IO.File.OpenRead(path)) {
                byte[] result = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0) {
                    int n = fsSource.Read(result, numBytesRead, numBytesToRead);

                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                return result;
            }
        }
    }
}

