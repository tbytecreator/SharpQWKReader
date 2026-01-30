using Z = System.IO.Compression;
using System.Text;

namespace SharpQWKReader.Web.Models
{
    public class BBSInfo
    {
        public string? BbsName { get; set; }
        public string? BbsPhone { get; set; }
        public string? BbsLocation { get; set; }
        public string? MailDoorReg { get; set; }
        public string? MailPacketCreationTime { get; set; }
        public string? SysopName { get; set; }
        public string? UserName { get; set; }
        public string? BbsId { get; set; }
        public Int32 MessagesInPacket { get; set; }
        public Int32 NumberOfForums { get; set; }
        public List<Forum>? Forums { get; set; }
    }

     public class Forum
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public int NumberOfMessages { get; set; }
    }
    public class MessagePointer
    {
        public ulong messageBytesLocation { get; set; }
         public string? StatusFlag { get; set; }
        public string? Number { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? To { get; set; }
        public string? From { get; set; }
        public string? Subject { get; set; }
        public string? ReferenceMessageNumber { get; set; }
    }

    public class Message
    {
        public string? StatusFlag { get; set; }
        public string? Number { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? To { get; set; }
        public string? From { get; set; }
        public string? Subject { get; set; }
        public string? Password { get; set; }
        public string? ReferenceMessageNumber { get; set; }
        public int MessageBlocks { get; set; }
        public string? DeleteFlag { get; set; }
        public string? ConferenceNumber { get; set; }
        public string? NumberInCurrentePacket { get; set; }
        public string? TagLineFlag { get; set; }
        public string? Body { get; set; }
        public ulong Index { get; set; }
    }

    public class Methods
    {
        public static void OpenQWKPacket(string packet, string tmpdir)
        {
            if (Directory.Exists(tmpdir))
            {
                var files = Directory.GetFiles(tmpdir);
                for (var i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
                Directory.Delete(tmpdir);
            }
            Directory.CreateDirectory(tmpdir);
            Z.ZipFile.ExtractToDirectory(packet, tmpdir);
        }

        public static string[] OpenControlDat(string tmpdir)
        {
            return File.ReadAllLines(tmpdir + "CONTROL.DAT", Encoding.ASCII);
        }

        public static byte[] OpenNDXFile(string tmpdir, string forumId)
        {
            var fileName = tmpdir + forumId + ".NDX";
            if (!File.Exists(fileName)) return Array.Empty<byte>();
            return File.ReadAllBytes(fileName);
        }

        public static BBSInfo GetBBSInfo(string tmpdir)
        {
            var bbsInfo = new BBSInfo();
            var lines = OpenControlDat(tmpdir);
            var line4 = lines[4].Split(','); 

            bbsInfo.BbsName = lines[0].Trim();
            bbsInfo.BbsLocation = lines[1].Trim();
            bbsInfo.BbsPhone = lines[2].Trim();
            bbsInfo.SysopName = lines[3].Trim();
            bbsInfo.MailDoorReg = line4[0].Trim();
            bbsInfo.BbsId = line4[1].Trim();
            bbsInfo.MailPacketCreationTime = lines[5].Trim();
            bbsInfo.UserName = lines[6].Trim();
            bbsInfo.NumberOfForums = Convert.ToInt32(lines[10].Trim());
            bbsInfo.MessagesInPacket = 0;
            bbsInfo.Forums = GetForuns(tmpdir,bbsInfo,lines);

            foreach (var forum in bbsInfo.Forums)
            {
                bbsInfo.MessagesInPacket = bbsInfo.MessagesInPacket+GetForumNumberOfMessages(tmpdir, forum.ID!);
            }

            return bbsInfo;
        }

        public static List<Forum> GetForuns(string tmpdir,BBSInfo bbsInfo,string[] lines)
        {
            var foruns = new List<Forum>();
            var numberOfConferences = bbsInfo.NumberOfForums;   
            var index = 11;

            for (int i=0;i<=numberOfConferences+1;i++)
            {
                var confNumber = lines[index].Trim();
                var confName = lines[index + 1].Trim();

                foruns.Add(new Forum()
                {
                    ID = confNumber,
                    Name = confName,
                    NumberOfMessages = GetForumNumberOfMessages(tmpdir, confNumber)
                });
                index += 2;
            }

            return foruns;
        }

        public static Int32 GetForumNumberOfMessages(string tmpdir, string forumId)
        {
            var pointers = OpenNDXFile(tmpdir, forumId);
            if (pointers.Length < 5) return 0;
            int NumOfMessages = pointers.Length / 5;
            return NumOfMessages;
        }
       
        public static List<MessagePointer> GetForumMessagePointers(string tmpdir, string forumId)
        {
            var result = new List<MessagePointer>();
            var fileName = tmpdir + forumId + ".NDX";

            if (!File.Exists(fileName)) return result;

            var fileContent = File.ReadAllBytes(fileName);
            var messagePointers = new List<MessagePointer>();

            for (int i = 0; i < fileContent.Length; i += 4)
            {
                var bytes = new byte[] { fileContent[i], fileContent[i + 1], fileContent[i + 2], fileContent[i + 3] };
                var offset = BitConverter.ToUInt32(bytes, 0) * 128;
                messagePointers.Add(new MessagePointer() { messageBytesLocation = offset });
            }

            return messagePointers;
        }

        public static Message GetMessage(string tmpdir, ulong messageNumber)
        {
            var indexFileName = tmpdir + "MESSAGES.DAT";
            var content = File.ReadAllBytes(indexFileName);

            var startPosition = messageNumber * 128;

            var statusFlag = Encoding.ASCII.GetString(content, (int)startPosition, 1);
            var number = Encoding.ASCII.GetString(content, (int)startPosition + 1, 8).Trim();
            var dateTime = Encoding.ASCII.GetString(content, (int)startPosition + 9, 20).Trim();

            var date = dateTime.Substring(0, 8);
            var time = dateTime.Substring(9, 8);

            var to = Encoding.ASCII.GetString(content, (int)startPosition + 31, 25).Trim();
            var from = Encoding.ASCII.GetString(content, (int)startPosition + 56, 25).Trim();
            var subject = Encoding.ASCII.GetString(content, (int)startPosition + 81, 25).Trim();
            var password = Encoding.ASCII.GetString(content, (int)startPosition + 106, 12).Trim();
            var referenceMessageNumber = Encoding.ASCII.GetString(content, (int)startPosition + 118, 8).Trim();
            var messageBlocks = Convert.ToInt32(Encoding.ASCII.GetString(content, (int)startPosition + 120, 6).Trim());
            var deleteFlag = Encoding.ASCII.GetString(content, (int)startPosition + 123, 1);
            var conferenceNumber = Encoding.ASCII.GetString(content, (int)startPosition + 116, 2).Trim();
            var numberInCurrentePacket = Encoding.ASCII.GetString(content, (int)startPosition + 126, 1);
            var tagLineFlag = Encoding.ASCII.GetString(content, (int)startPosition + 127, 1);

            var message = new Message()
            {
                StatusFlag = statusFlag,
                Number = number,
                Date = date,
                Time = time,
                To = to,
                From = from,
                Subject = subject,
                Password = password,
                ReferenceMessageNumber = referenceMessageNumber,
                MessageBlocks = messageBlocks,
                DeleteFlag = deleteFlag,
                ConferenceNumber = conferenceNumber,
                NumberInCurrentePacket = numberInCurrentePacket,
                TagLineFlag = tagLineFlag,
                Index = messageNumber
            };

            var bodyStartPostion = messageNumber * 128 + 128;
            var bodyLength = messageBlocks * 128;
            var body = Encoding.ASCII.GetString(content, (int)bodyStartPostion, (int)bodyLength);
            message.Body = body.Trim();

            return message;
        }
    }
}
