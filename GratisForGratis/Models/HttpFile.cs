using System.IO;
using System.Web;

namespace GratisForGratis.Models
{
    class HttpFile : HttpPostedFileBase
    {
        Stream stream;
        string contentType;
        string fileName;

        public HttpFile(string fileName)
        {
            this.stream = System.IO.File.OpenRead(fileName);
            this.fileName = fileName;
            this.contentType = MimeMapping.GetMimeMapping(fileName);
        }

        public HttpFile(Stream stream, string contentType, string fileName)
        {
            this.stream = stream;
            this.contentType = contentType;
            this.fileName = fileName;
        }

        public override int ContentLength
        {
            get { return (int)stream.Length; }
        }

        public override string ContentType
        {
            get { return contentType; }
        }

        public override string FileName
        {
            get { return fileName; }
        }

        public override Stream InputStream
        {
            get { return stream; }
        }

        public override void SaveAs(string filename)
        {
            using (var file = System.IO.File.Open(filename, FileMode.CreateNew))
                stream.CopyTo(file);
        }
    }
}
