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
        public enum ArchivedType : byte
        {
            File,
            Metadata
        }

        public Archiver(string fileName, ArchiverMode mode, DateTime? createdDateTime = null)
        {
            m_fileName = fileName;
            m_createdDateTime = (createdDateTime == null ? DateTime.Now : (DateTime)createdDateTime);
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

        public void WriteMetadataString(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            WriteMetadataArray(dataBytes);
        }
        public void WriteMetadataArray(byte[] data)
        {
            if (m_mode != ArchiverMode.Write) throw new InvalidOperationException("In order to write, mode must be set to ArchiverMode.Write");

            m_stream.WriteByte((byte)ArchivedType.Metadata);

            if (data == null)
            {
                m_stream.Write(BitConverter.GetBytes(0), 0, 4);
            }
            else
            {
                m_stream.Write(BitConverter.GetBytes(data.Length), 0, 4);
                m_stream.Write(data, 0, data.Length);
            }
        }
        public void WriteFile(string filePath)
        {
            if (m_mode != ArchiverMode.Write) throw new InvalidOperationException("In order to write, mode must be set to ArchiverMode.Write");

            m_stream.WriteByte((byte)ArchivedType.File);

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
        public string ReadMetadataString()
        {
            return Encoding.UTF8.GetString(ReadMetadataArray());
        }
        public byte[] ReadMetadataArray()
        {
            if (m_mode != ArchiverMode.Read) throw new InvalidOperationException("In order to read, mode must be set to ArchiverMode.Read");

            ArchivedType type = ReadType();
            if (type != ArchivedType.Metadata) throw new InvalidDataException("Next archived type is not Metadata. It is: " + type.ToString());

            m_stream.Read(m_buffer, 0, 4);
            int length = BitConverter.ToInt32(m_buffer, 0);

            byte[] result = new byte[length];

            m_stream.Read(result, 0, length);

            return result;
        }
        public void ReadFile(string resultDirectory)
        {
            if (m_mode != ArchiverMode.Read) throw new InvalidOperationException("In order to read, mode must be set to ArchiverMode.Read");

            ArchivedType type = ReadType();
            if (type != ArchivedType.File) throw new InvalidDataException("Next archived type is not File. It is: " + type.ToString());

            m_stream.Read(m_buffer, 0, 4);
            int nameLength = BitConverter.ToInt32(m_buffer, 0);

            byte[] nameBytes = new byte[nameLength];
            m_stream.Read(nameBytes, 0, nameBytes.Length);
            string name = Encoding.UTF8.GetString(nameBytes);
            string fullPath = resultDirectory + @"\" + name;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                m_stream.Read(m_buffer, 0, 8);
                long remainingFileLength = BitConverter.ToInt64(m_buffer, 0);

                int readCount = -1;
                while ((readCount = m_stream.Read(m_buffer, 0, (int)Math.Min((long)m_buffer.Length, remainingFileLength))) > 0)
                {
                    fileStream.Write(m_buffer, 0, readCount);
                    remainingFileLength -= readCount;
                }

                fileStream.Flush();
                fileStream.Close();
            }
        }

        public ArchivedType NextAvailableType()
        {
            if (m_mode != ArchiverMode.Read) throw new InvalidOperationException("In order to read, mode must be set to ArchiverMode.Read");

            ArchivedType type = ReadType();
            m_stream.Position -= 1;

            return type;
        }
        protected ArchivedType ReadType()
        {
            return (ArchivedType)((byte)m_stream.ReadByte());
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
