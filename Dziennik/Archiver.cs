using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dziennik
{
    public class Archiver : IDisposable
    {
        public enum ArchiverMode
        {
            Write,
            Read,
        }

        public Archiver(string fileName, DateTime createdDateTime, ArchiverMode mode)
        {
            m_fileName = fileName;
            m_createdDateTime = createdDateTime;
            m_mode = mode;
        }

        private bool m_disposed = false;
        private FileStream m_stream;
        private byte[] m_buffer = new byte[4096];

        private string m_fileName;
        public string FileName
        {
            get { return m_fileName; }
        }

        private DateTime m_createdDateTime;
        public DateTime CreatedDateTime
        {
            get { return m_createdDateTime; }
        }
        
        private ArchiverMode m_mode;
        public ArchiverMode Mode
        {
            get { return m_mode; }
        }

        private bool m_started = false;
        public bool Started
        {
            get { return m_started; }
        }

        public void Start()
        {
            m_started = true;

            switch(m_mode)
            {
                case ArchiverMode.Write: StartWrite(); break;
                case ArchiverMode.Read: StartRead(); break;
            }
        }
        public void End()
        {
            m_started = false;

            m_stream.Close();
            m_stream.Dispose();
            m_stream = null;
        }

        protected virtual void StartWrite()
        {
            m_stream = new FileStream(m_fileName, FileMode.OpenOrCreate);

            byte[] createdDateTimeResult = BitConverter.GetBytes(m_createdDateTime.ToBinary());
            m_stream.Write(createdDateTimeResult, 0, 8);
        }
        protected virtual void StartRead()
        {
            m_stream = new FileStream(m_fileName, FileMode.Open);

            byte[] createdDateTimeResult = new byte[8];
            m_stream.Read(createdDateTimeResult, 0, 8);
            m_createdDateTime = DateTime.FromBinary(BitConverter.ToInt64(createdDateTimeResult, 0));
        }

        public void WriteFile(string filePath)
        {
            if (m_mode != ArchiverMode.Write) throw new InvalidOperationException("In order to write, mode must be set to ArchiverMode.Write");

            string name = Path.GetFileName(filePath);
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            m_stream.Write(BitConverter.GetBytes(nameBytes.Length), 0, 4);
            m_stream.Write(nameBytes, 0, nameBytes.Length);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                m_stream.Write(BitConverter.GetBytes(fileStream.Length), 0, 8);

                int readCount = -1;
                while ((readCount = fileStream.Read(m_buffer, 0, m_buffer.Length)) > 0)
                {
                    m_stream.Write(m_buffer, 0, readCount);
                }

                fileStream.Close();
            }
        }
        public void ReadFile(string resultDirectory)
        {
            if (m_mode != ArchiverMode.Read) throw new InvalidOperationException("In order to read, mode must be set to ArchiverMode.Read");

            m_stream.Read(m_buffer, 0, 4);
            int nameLength = BitConverter.ToInt32(m_buffer, 0);

            byte[] nameBytes = new byte[nameLength];
            string name = Encoding.UTF8.GetString(nameBytes);
            string fullPath = resultDirectory + @"\" + name;

            using (FileStream fileStream  = new FileStream(fullPath,FileMode.OpenOrCreate))
            {
                m_stream.Read(m_buffer, 0, 8);
                long remainingFileLength = BitConverter.ToInt64(m_buffer, 0);

                int readCount = -1;
                while ((readCount = fileStream.Read(m_buffer, 0, (int)Math.Min((long)m_buffer.Length, remainingFileLength))) > 0)
                {
                    fileStream.Write(m_buffer, 0, readCount);
                }

                fileStream.Close();
            }
        }
        public bool IsEndOfArchive()
        {
            if (m_mode != ArchiverMode.Read) throw new InvalidOperationException("In order to read, mode must be set to ArchiverMode.Read");
            return m_stream.Position == m_stream.Length;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    //managed
                    if (m_started) End();
                }

                //unmanaged
                m_disposed = true;
            }
        }
        ~Archiver()
        {
            Dispose(false);
        }
    }
}
