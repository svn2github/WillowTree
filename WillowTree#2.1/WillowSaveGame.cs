﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X360.IO;
using X360.STFS;

namespace WillowTree
{

    // Fun fun fun time. Okay, well after I decided to ditch using DJsIO I ran into a number of problems.
    // 90% of them revolved around Microsofts total lack of Big Endian support. So I had to add my own, 
    // and by that I mean I snatched some code off of Google and twisted it to work for WillowTree. Well,
    // it works doesn't it? Aaaaaaaanyway, I really want to fix up the opening/saving process so that you
    // can edit specific parts of the save without requiring a complete rebuild. For example, if you just 
    // (want to) edit a weapon, it rebuilds the weapon list only.

    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

    public class WillowSaveGame
    {
        public static byte[] ReadBytes(BinaryReader reader, int fieldSize, ByteOrder byteOrder)
        {
            byte[] bytes = reader.ReadBytes(fieldSize);
            if (bytes.Length != fieldSize)
                throw new EndOfStreamException();

            if (BitConverter.IsLittleEndian)
            {
                if (byteOrder == ByteOrder.BigEndian)
                    Array.Reverse(bytes);
            }
            else
            {
                if (byteOrder == ByteOrder.LittleEndian)
                    Array.Reverse(bytes);
            }

            return bytes;
        }
        public static byte[] ReadBytes(byte[] inBytes, int fieldSize, ByteOrder byteOrder)
        {
            if (inBytes.Length < fieldSize)
                throw new ArgumentException("Input bytes length is less than the field size.");
            byte[] bytes = new byte[fieldSize];
            if (byteOrder == ByteOrder.LittleEndian)
                return inBytes;
            else
            {
                int f = 0;
                for (int i = fieldSize - 1; i > -1; i--)
                {
                    bytes[i] = inBytes[f];
                    f++;
                }
                return bytes;
            }
        }

        public static int ReadInt32(BinaryReader reader, ByteOrder Endian)
        {
            return BitConverter.ToInt32(ReadBytes(reader, sizeof(int), Endian), 0);
        }
        public static short ReadInt16(BinaryReader reader, ByteOrder Endian)
        {
            return BitConverter.ToInt16(ReadBytes(reader, sizeof(short), Endian), 0);
        }
        public static List<int> ReadListInt32(BinaryReader reader, ByteOrder Endian)
        {
            int count = ReadInt32(reader, Endian);
            List<int> list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int value = ReadInt32(reader, Endian);
                list.Add(value);
            }
            return list;
        }

        public static void WriteInt32(int inInt, BinaryWriter writer, ByteOrder Endian)
        {
            writer.Write(BitConverter.ToInt32(ReadBytes(BitConverter.GetBytes(inInt), sizeof(int), Endian), 0));
        }
        public static void WriteInt16(short inInt, BinaryWriter writer, ByteOrder Endian)
        {
            writer.Write(ReadBytes(BitConverter.GetBytes((short)inInt), sizeof(short), Endian));
        }
        public static byte[] Int32ToBtyes(int inInt, ByteOrder Endian)
        {
            return ReadBytes(BitConverter.GetBytes(inInt), sizeof(int), Endian);
        }
        public static byte[] Int16ToBtyes(int inInt, ByteOrder Endian)
        {
            return ReadBytes(BitConverter.GetBytes((short)inInt), 2, Endian);
        }
        ///<summary>Reads a string in the format used by the WSGs</summary>
        public string ReadString(BinaryReader reader, ByteOrder Endian)
        {
            int TempLengthValue = ReadInt32(reader, Endian);
            if (TempLengthValue == 0)
                return string.Empty;

            string value;

            // Read string data (either ASCII or Unicode).
            if (TempLengthValue < 0)
            {
                // Convert the length value into a unicode byte count.
                TempLengthValue = -TempLengthValue * 2;

                // Read the byte data (and ensure that the number of bytes
                // read matches the number of bytes it was supposed to read--BinaryReader may not return the number of bytes read).
                byte[] data = reader.ReadBytes(TempLengthValue);
                if (data.Length != TempLengthValue)
                    throw new EndOfStreamException();

                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                byte[] data = reader.ReadBytes(TempLengthValue);
                if (data.Length != TempLengthValue)
                    throw new EndOfStreamException();

                value = Encoding.ASCII.GetString(data);
            }

            // Look for the null terminator character. If not found, return
            // the entire string.
            int nullTerminatorIndex = value.IndexOf('\0');
            if (nullTerminatorIndex < 0)
                return value;

            // If the null terminator is the first character in the string,
            // then return an empty string (small reference optimization).
            if (nullTerminatorIndex == 0)
                return string.Empty;

            // Return a portion of the string, excluding the null terminator
            // and any character after the null terminator.
            return value.Substring(0, nullTerminatorIndex);
        }

        public static void Write(BinaryWriter writer, int outInt, ByteOrder Endian)
        {
            writer.Write(Int32ToBtyes(outInt, Endian));
        }
        public static void Write(BinaryWriter writer, byte[] outBytes, ByteOrder Endian)
        {
            writer.Write(outBytes);
        }
        public static void Write16(BinaryWriter writer, int outInt, ByteOrder Endian)
        {
            writer.Write(Int16ToBtyes(outInt, Endian));
        }

        //Checks if the byte/array is null.
        public static bool isNotNull<T>(T[] array)
        {
            return array != null && array.Length > 0;
        }

        public bool WSGEndian;
        public ByteOrder EndianWSG;
        public BinaryReader WSGReader;

        public string Platform;
        public string OpenedWSG;
        public bool ContainsRawData;
        //General Info
        public string MagicHeader;
        public int VersionNumber;
        public string PLYR;
        public int RevisionNumber;
        public string Class;
        public int Level;
        public int Experience;
        public int SkillPoints;
        public int Unknown1;
        public int Cash;
        public int FinishedPlaythrough1;
        //Skill Arrays
        public int NumberOfSkills;
        public string[] SkillNames;
        public int[] LevelOfSkills;
        public int[] ExpOfSkills;
        public int[] InUse;
        //Vehicle Info
        public int Vehi1Color;
        public int Vehi2Color;
        public int Vehi1Type; // 0 = rocket, 1 = machine gun
        public int Vehi2Type;
        //Ammo Pool Arrays
        public int NumberOfPools;
        public string[] ResourcePools;
        public string[] AmmoPools;
        public float[] RemainingPools;
        public int[] PoolLevels;
        //Item Arrays
        public int NumberOfItems;
        public List<List<string>> ItemStrings;
        public List<List<int>> ItemValues;
        //Backpack Info
        public int BackpackSize;
        public int EquipSlots;
        //Weapon Arrays
        public int NumberOfWeapons;
        public List<List<string>> WeaponStrings = new List<List<string>>();
        public List<List<int>> WeaponValues = new List<List<int>>();
        //Look, stuff!
        public int ChallengeDataBlockLength;
        public int ChallengeDataBlockId;
        public int ChallengeDataLength;
        public Int16 ChallengeDataEntries;
        public struct ChallengeDataEntry
        {
            public Int16 Id;
            public Byte TypeId;
            public Int32 Value;
        }
        List<ChallengeDataEntry> Challenges;

        public byte[] ChallengeData;
        public int TotalLocations;
        public string[] LocationStrings;
        public string CurrentLocation;
        public int[] SaveInfo1to5 = new int[5];
        public int SaveNumber;
        public int[] SaveInfo7to10 = new int[4];
        //Quest Arrays/Info
        public string CurrentQuest = "None";
        public int TotalPT1Quests;
        public string[] PT1Strings;
        public int[,] PT1Values;
        public string[,] PT1Subfolders;
        public int UnknownPT1QuestValue;
        public int TotalPT2Quests;
        public string SecondaryQuest = "None";
        public string[] PT2Strings;
        public int[,] PT2Values;
        public string[,] PT2Subfolders;
        public int UnknownPT2QuestValue;
        //More unknowns and color info.
        public int TotalPlayTime;
        public string LastPlayedDate;
        public string CharacterName;
        public int Color1;
        public int Color2;
        public int Color3;
        public int Unknown2;
        public List<int> PromoCodesUsed;
        public List<int> PromoCodesRequiringNotification;
        //Echo Info
        public int NumberOfEchoLists;
        public int EchoIndex0;
        public int NumberOfEchos;
        public string[] EchoStrings;
        public int[,] EchoValues;
        public int EchoIndex1;
        public int NumberOfEchosPT2;
        public string[] EchoStringsPT2;
        public int[,] EchoValuesPT2;
        //public byte[] MiscData;

        // Temporary lists used for primary pack data when the inventory is split
        public List<List<string>> ItemStrings1;
        public List<List<int>> ItemValues1;
        public List<List<string>> WeaponStrings1;
        public List<List<int>> WeaponValues1;
        // Temporary lists used for primary pack data when the inventory is split
        public List<List<string>> ItemStrings2;
        public List<List<int>> ItemValues2;
        public List<List<string>> WeaponStrings2;
        public List<List<int>> WeaponValues2;

        public class DLC_Data
        {
            public const int Section1Id = 0x43211234;
            public const int Section2Id = 0x02151984;
            public const int Section3Id = 0x32235947;
            public const int Section4Id = 0x234BA901;

            public bool HasSection1;
            public bool HasSection2;
            public bool HasSection3;
            public bool HasSection4;

            public List<DLCSection> DataSections;

            public int DLC_Size;

            // DLC Section 1 Data (bank data)
            public byte DLC_Unknown1;  // Read only flag. Always resets to 1 in ver 1.41.  Probably CanAccessBank.
            public int BankSize;
            // DLC Section 2 Data (don't know)
            public int DLC_Unknown2; // All four of these are boolean flags.
            public int DLC_Unknown3; // If you set them to any value except 0
            public int DLC_Unknown4; // the game will rewrite them as 1.
            public int SkipDLC2Intro;//
            // DLC Section 3 Data (related to the level cap.  removing this section will delevel your character to 50)
            public byte DLC_Unknown5;  // Read only flag. Always resets to 1 in ver 1.41.  Probably CanExceedLevel50
            // DLC Section 4 Data (DLC backpack)
            public byte SecondaryPackEnabled;  // Read only flag. Always resets to 1 in ver 1.41.
            public int NumberOfItems;
            public List<List<string>> ItemStrings = new List<List<string>>();
            public List<List<int>> ItemValues = new List<List<int>>();
            public int NumberOfWeapons;
            public List<List<string>> WeaponStrings = new List<List<string>>();
            public List<List<int>> WeaponValues = new List<List<int>>();

            public int TotalItems;
            public List<List<string>> ItemParts = new List<List<string>>();
            public List<int> ItemQuantity = new List<int>();
            public List<int> ItemLevel = new List<int>();
            public List<int> ItemQuality = new List<int>();
            public List<int> ItemEquipped = new List<int>();

            public int TotalWeapons;
            public List<List<string>> WeaponParts = new List<List<string>>();
            public List<int> WeaponAmmo = new List<int>();
            public List<int> WeaponLevel = new List<int>();
            public List<int> WeaponQuality = new List<int>();
            public List<int> WeaponEquippedSlot = new List<int>();
        }

        public DLC_Data DLC = new DLC_Data();

        public class DLCSection
        {
            public int Id;
            public byte[] RawData;
            public byte[] BaseData; // used temporarily in SaveWSG to store the base data for a section as a byte array
        }

        //Xbox 360 only
        public long ProfileID;
        public byte[] DeviceID;
        public byte[] CON_Image;
        public string Title_Display;
        public string Title_Package;
        public uint TitleID = 1414793191;

        ///<summary>Reports back the expected platform this WSG was created on.</summary>
        public string WSGType(byte[] InputWSG)
        {
            DJsIO SaveReader = new DJsIO(InputWSG, true);
            string Magic = SaveReader.ReadString(3, StringForm.ASCII, StringRead.Defined);

            if (Magic == "CON")
            {
                SaveReader.ReadBytes(53245);
                string SecondaryMagic = SaveReader.ReadString(3, StringForm.ASCII, StringRead.Defined);
                SaveReader.Close();

                if (SecondaryMagic == "WSG")
                    return "X360";
                else
                    return "Not WSG";
            }
            else if (Magic == "WSG")
            {
                int WSG_Version = SaveReader.ReadInt32();
                SaveReader.Close();
                if (WSG_Version == 2)
                    return "PS3";
                else if (WSG_Version == 33554432)
                    return "PC";
                else
                    return "unknown";
            }
            else
            {
                SaveReader.Close();
                return "Not WSG";
            }

        }
        ///<summary>Extracts a WSG from a CON.</summary>
        public byte[] WSGExtract(string InputX360File)
        {
            try
            {
                STFSPackage CON = new STFSPackage(new DJsIO(InputX360File, DJFileMode.Open, true), new X360.Other.LogRecord());
                DJsIO Extract = new DJsIO(true);
                //CON.FileDirectory[0].Extract(Extract);
                ProfileID = CON.Header.ProfileID;
                DeviceID = CON.Header.DeviceID;
                //DJsIO Save = new DJsIO("C:\\temp.sav", DJFileMode.Create, true);
                //Save.Write(Extract.ReadStream());
                //Save.Close();
                //byte[] nom = CON.GetFile("SaveGame.sav").GetEntryData(); 
                return CON.GetFile("SaveGame.sav").GetTempIO(true).ReadStream();
            }
            catch
            {
                try
                {
                    DJsIO Manual = new DJsIO(InputX360File, DJFileMode.Open, true);
                    Manual.ReadBytes(881);
                    ProfileID = Manual.ReadInt64();
                    Manual.ReadBytes(132);
                    DeviceID = Manual.ReadBytes(20);
                    Manual.ReadBytes(48163);
                    int size = Manual.ReadInt32();
                    Manual.ReadBytes(4040);
                    return Manual.ReadBytes(size);
                }
                catch { return null; }
            }
        }
        ///<summary>Converts a file to a byte[] for use with ReadWSG</summary>
        public void OpenWSG(string InputFile)
        {
            //try
            //{
            DJsIO Reader = new DJsIO(InputFile, DJFileMode.Open, true);
            Platform = WSGType(Reader.ReadStream());

            if (Platform == "X360")
            {
                Reader.Close();
                ReadWSG(WSGExtract(InputFile));

            }
            else if (Platform == "PS3")
            {
                ReadWSG(Reader.ReadStream());
                Reader.Close();
            }
            else if (Platform == "PC")
            {
                ReadWSG(Reader.ReadStream());
                Reader.Close();
            }
            OpenedWSG = InputFile;
            //}

            //catch { }

        }
        ///<summary>Reads and decompiles the contents of a WSG</summary>
        public void ReadWSG(byte[] FileArray)
        {
            bool isBigEndian;
            BinaryReader TestReader = new BinaryReader(new MemoryStream(FileArray));

            ContainsRawData = false;
            MagicHeader = new string(TestReader.ReadChars(3));
            VersionNumber = TestReader.ReadInt32();

            if (VersionNumber == 0x2000000)
            {
                VersionNumber = 2;
                isBigEndian = BitConverter.IsLittleEndian;
            }
            else
                isBigEndian = !BitConverter.IsLittleEndian;
            EndianWSG = (isBigEndian ? ByteOrder.BigEndian : ByteOrder.LittleEndian);
            WSGEndian = isBigEndian;

            PLYR = new string(TestReader.ReadChars(4));
            RevisionNumber = ReadInt32(TestReader, EndianWSG);
            Class = ReadString(TestReader);
            Level = ReadInt32(TestReader, EndianWSG);
            Experience = ReadInt32(TestReader, EndianWSG);
            SkillPoints = ReadInt32(TestReader, EndianWSG);
            Unknown1 = ReadInt32(TestReader, EndianWSG);
            Cash = ReadInt32(TestReader, EndianWSG);
            FinishedPlaythrough1 = ReadInt32(TestReader, EndianWSG);
            NumberOfSkills = ReadInt32(TestReader, EndianWSG);
            Skills(TestReader, NumberOfSkills);
            Vehi1Color = ReadInt32(TestReader, EndianWSG);
            Vehi2Color = ReadInt32(TestReader, EndianWSG);
            Vehi1Type = ReadInt32(TestReader, EndianWSG);
            Vehi2Type = ReadInt32(TestReader, EndianWSG);
            NumberOfPools = ReadInt32(TestReader, EndianWSG);
            Ammo(TestReader, NumberOfPools);
            ItemStrings = new List<List<string>>();
            ItemValues = new List<List<int>>();
            NumberOfItems = ReadInt32(TestReader, EndianWSG);
            Items(TestReader, NumberOfItems);
            BackpackSize = ReadInt32(TestReader, EndianWSG);
            EquipSlots = ReadInt32(TestReader, EndianWSG);
            WeaponStrings = new List<List<string>>();
            WeaponValues = new List<List<int>>();
            NumberOfWeapons = ReadInt32(TestReader, EndianWSG);
            Weapons(TestReader, NumberOfWeapons);

            ChallengeDataBlockLength = ReadInt32(TestReader, EndianWSG);
            ChallengeDataBlockId = ReadInt32(TestReader, EndianWSG);
            ChallengeDataLength = ReadInt32(TestReader, EndianWSG);
            ChallengeDataEntries = ReadInt16(TestReader, EndianWSG);
            Challenges = new List<ChallengeDataEntry>();
            for (int i = 0; i < ChallengeDataEntries; i++)
            {
                ChallengeDataEntry challenge = new ChallengeDataEntry();
                challenge.Id = ReadInt16(TestReader, EndianWSG);
                challenge.TypeId = TestReader.ReadByte();
                challenge.Value = ReadInt32(TestReader, EndianWSG);
                Challenges.Add(challenge);
            }

            TotalLocations = ReadInt32(TestReader, EndianWSG);
            Locations(TestReader, TotalLocations);
            CurrentLocation = ReadString(TestReader);
            SaveInfo1to5[0] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[1] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[2] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[3] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[4] = ReadInt32(TestReader, EndianWSG);
            SaveNumber = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[0] = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[1] = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[2] = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[3] = ReadInt32(TestReader, EndianWSG);
            CurrentQuest = ReadString(TestReader, EndianWSG);
            TotalPT1Quests = ReadInt32(TestReader, EndianWSG);
            PT1Quests(TestReader, TotalPT1Quests);
            UnknownPT1QuestValue = ReadInt32(TestReader, EndianWSG);
            SecondaryQuest = ReadString(TestReader, EndianWSG);
            TotalPT2Quests = ReadInt32(TestReader, EndianWSG);
            PT2Quests(TestReader, TotalPT2Quests); //Saves playthrough 2 quests as arrays of strings and ints (15 values, -1 for none if not used)
            UnknownPT2QuestValue = ReadInt32(TestReader, EndianWSG); //Is either 2 or 0
            ReadString(TestReader); //Z0_Missions.Missions.M_IntroStateSaver
            ReadInt32(TestReader, EndianWSG); //1
            ReadString(TestReader); //Z0_Missions.Missions.M_IntroStateSaver
            ReadInt32(TestReader, EndianWSG); //2
            ReadInt32(TestReader, EndianWSG); //0
            ReadInt32(TestReader, EndianWSG); //0
            ReadInt32(TestReader, EndianWSG); //0
            TotalPlayTime = ReadInt32(TestReader, EndianWSG);
            LastPlayedDate = ReadString(TestReader);
            CharacterName = ReadString(TestReader);
            Color1 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Color2 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Color3 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Unknown2 = ReadInt32(TestReader, EndianWSG);
            PromoCodesUsed = ReadListInt32(TestReader, EndianWSG);
            PromoCodesRequiringNotification = ReadListInt32(TestReader, EndianWSG);
            NumberOfEchoLists = ReadInt32(TestReader, EndianWSG);

            EchoIndex0 = ReadInt32(TestReader, EndianWSG);
            NumberOfEchos = ReadInt32(TestReader, EndianWSG);
            Echos(TestReader, NumberOfEchos); //One string, two identical int32s

            if (NumberOfEchoLists > 1) // there can be 1 or 2 echo lists
            {
                EchoIndex1 = ReadInt32(TestReader, EndianWSG);
                NumberOfEchosPT2 = ReadInt32(TestReader, EndianWSG);
                EchosPT2(TestReader, NumberOfEchosPT2);
            }

            DLC.DataSections = new List<DLCSection>();
            DLC.DLC_Size = ReadInt32(TestReader, EndianWSG);

            if (DLC.DLC_Size > 0)
            {
                int RemainingBytes = DLC.DLC_Size;
                while (RemainingBytes > 0)
                {
                    DLCSection Section = new DLCSection();
                    Section.Id = ReadInt32(TestReader, EndianWSG);
                    int SectionLength = ReadInt32(TestReader, EndianWSG);
                    long SectionStartPos = (int)TestReader.BaseStream.Position;
                    switch (Section.Id)
                    {
                        case DLC_Data.Section1Id:
                            DLC.HasSection1 = true;
                            DLC.DLC_Unknown1 = TestReader.ReadByte();
                            DLC.BankSize = ReadInt32(TestReader, EndianWSG);
                            // The bank item data which comes next is stored in RawData below
                            break;
                        case DLC_Data.Section2Id:
                            DLC.HasSection2 = true;
                            DLC.DLC_Unknown2 = ReadInt32(TestReader, EndianWSG);
                            DLC.DLC_Unknown3 = ReadInt32(TestReader, EndianWSG);
                            DLC.DLC_Unknown4 = ReadInt32(TestReader, EndianWSG);
                            DLC.SkipDLC2Intro = ReadInt32(TestReader, EndianWSG);
                            break;
                        case DLC_Data.Section3Id:
                            DLC.HasSection3 = true;
                            DLC.DLC_Unknown5 = TestReader.ReadByte();
                            break;
                        case DLC_Data.Section4Id:
                            DLC.HasSection4 = true;
                            DLC.SecondaryPackEnabled = TestReader.ReadByte();
                            DLC.NumberOfItems = ReadInt32(TestReader, EndianWSG);
                            Items(TestReader, DLC.NumberOfItems);
                            NumberOfItems += DLC.NumberOfItems;
                            DLC.NumberOfWeapons = ReadInt32(TestReader, EndianWSG);
                            Weapons(TestReader, DLC.NumberOfWeapons);
                            NumberOfWeapons += DLC.NumberOfWeapons;
                            break;
                        default:
                            break;
                    }
                    // I don't pretend to know if any of the DLC sections will ever expand
                    // and store more data.  RawData stores any extra data at the end of
                    // the known data in any section and stores the entirety of sections 
                    // with unknown ids in a buffer in its raw byte order dependent form.
                    int RawDataCount = SectionLength - (int)(TestReader.BaseStream.Position - SectionStartPos);
                    Section.RawData = TestReader.ReadBytes(RawDataCount);
                    if (RawDataCount > 0)
                        ContainsRawData = true;
                    RemainingBytes -= SectionLength + 8;
                    DLC.DataSections.Add(Section);
                }
            }
            TestReader.Close();
        }
        private void Skills(BinaryReader DJsIO, int NumOfSkills)
        {
            string[] TempSkillNames = new string[NumOfSkills];
            int[] TempLevelOfSkills = new int[NumOfSkills];
            int[] TempExpOfSkills = new int[NumOfSkills];
            int[] TempInUse = new int[NumOfSkills];

            for (int Progress = 0; Progress < NumOfSkills; Progress++)
            {
                TempSkillNames[Progress] = ReadString(DJsIO);
                TempLevelOfSkills[Progress] = ReadInt32(DJsIO, EndianWSG);
                TempExpOfSkills[Progress] = ReadInt32(DJsIO, EndianWSG);
                TempInUse[Progress] = ReadInt32(DJsIO, EndianWSG);
            }
            SkillNames = TempSkillNames;
            LevelOfSkills = TempLevelOfSkills;
            ExpOfSkills = TempExpOfSkills;
            InUse = TempInUse;
        } //Ignore all of the "DJsIO", I was just too lazy to rename them after I removed most of my dependence on the X360 DLL.
        private void Ammo(BinaryReader DJsIO, int NumOfPools)
        {
            string[] TempResourcePools = new string[NumOfPools];
            string[] TempAmmoPools = new string[NumOfPools];
            float[] TempRemainingPools = new float[NumOfPools];
            int[] TempPoolLevels = new int[NumOfPools];

            for (int Progress = 0; Progress < NumOfPools; Progress++)
            {
                TempResourcePools[Progress] = ReadString(DJsIO);
                TempAmmoPools[Progress] = ReadString(DJsIO);
                TempRemainingPools[Progress] = DJsIO.ReadSingle();
                TempPoolLevels[Progress] = ReadInt32(DJsIO, EndianWSG);
            }
            ResourcePools = TempResourcePools;
            AmmoPools = TempAmmoPools;
            RemainingPools = TempRemainingPools;
            PoolLevels = TempPoolLevels;
        }
        private void Items(BinaryReader reader, int NumOfItems)
        {
            for (int Progress = 0; Progress < NumOfItems; Progress++)
            {
                List<string> strings = new List<string>();
                for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                    strings.Add(ReadString(reader));
                ItemStrings.Add(strings);

                List<int> values = new List<int>();
                Int32 Value1 = ReadInt32(reader, EndianWSG);
                int tempLevelQuality = ReadInt32(reader, EndianWSG);
                Int16 Quality = (Int16)(tempLevelQuality % 65536);
                Int16 Level = (Int16)(tempLevelQuality / 65536);
                Int32 EquippedSlot = ReadInt32(reader, EndianWSG);
                values.Add(Value1);
                values.Add(Quality);
                values.Add(EquippedSlot);
                values.Add(Level);
                ItemValues.Add(values);
            }
        }
        private void Weapons(BinaryReader reader, int NumOfWeapons)
        {
            for (int Progress = 0; Progress < NumOfWeapons; Progress++)
            {
                List<string> strings = new List<string>();
                for (int TotalStrings = 0; TotalStrings < 14; TotalStrings++)
                    strings.Add(ReadString(reader));
                WeaponStrings.Add(strings);

                List<int> values = new List<int>();
                Int32 AmmoCount = ReadInt32(reader, EndianWSG);
                int tempLevelQuality = ReadInt32(reader, EndianWSG);
                Int16 Level = (Int16)(tempLevelQuality / 65536);
                Int16 Quality = (Int16)(tempLevelQuality % 65536);
                Int32 EquippedSlot = ReadInt32(reader, EndianWSG);
                values.Add(AmmoCount);
                values.Add(Quality);
                values.Add(EquippedSlot);
                values.Add(Level);
                WeaponValues.Add(values);
            }
        }

        private void PT1Quests(BinaryReader DJsIO, int NumOfQuests)
        {
            string[] TempQuestStrings = new string[NumOfQuests];
            int[,] TempQuestValues = new int[NumOfQuests, 9];
            string[,] TempQuestSubfolders = new string[NumOfQuests, 5];

            for (int Progress = 0; Progress < NumOfQuests; Progress++)
            {
                TempQuestStrings[Progress] = ReadString(DJsIO);
                for (int TotalValues = 0; TotalValues < 4; TotalValues++)
                    TempQuestValues[Progress, TotalValues] = ReadInt32(DJsIO, EndianWSG);

                if (TempQuestValues[Progress, 3] > 0)
                {
                    for (int ExtraValues = 0; ExtraValues < TempQuestValues[Progress, 3]; ExtraValues++)
                    {
                        TempQuestSubfolders[Progress, ExtraValues] = ReadString(DJsIO);
                        TempQuestValues[Progress, ExtraValues + 4] = ReadInt32(DJsIO, EndianWSG);
                    }
                }
                for (int FillGaps = TempQuestValues[Progress, 3] + 4; FillGaps < 9; FillGaps++)
                    TempQuestValues[Progress, FillGaps] = 0;
            }
            if (CurrentQuest == "None") CurrentQuest = TempQuestStrings[0];
            PT1Strings = TempQuestStrings;
            PT1Values = TempQuestValues;
            PT1Subfolders = TempQuestSubfolders;
        }
        private void PT2Quests(BinaryReader DJsIO, int NumOfQuests)
        {
            string[] TempQuestStrings = new string[NumOfQuests];
            int[,] TempQuestValues = new int[NumOfQuests, 9];
            string[,] TempQuestSubfolders = new string[NumOfQuests, 5];

            for (int Progress = 0; Progress < NumOfQuests; Progress++)
            {
                TempQuestStrings[Progress] = ReadString(DJsIO);

                for (int TotalValues = 0; TotalValues < 4; TotalValues++)
                    TempQuestValues[Progress, TotalValues] = ReadInt32(DJsIO, EndianWSG);

                if (TempQuestValues[Progress, 3] > 0)
                {
                    for (int ExtraValues = 0; ExtraValues < TempQuestValues[Progress, 3]; ExtraValues++)
                    {
                        TempQuestSubfolders[Progress, ExtraValues] = ReadString(DJsIO);
                        TempQuestValues[Progress, ExtraValues + 4] = ReadInt32(DJsIO, EndianWSG);
                    }
                    for (int FillGaps = TempQuestValues[Progress, 3] + 4; FillGaps < 9; FillGaps++)
                        TempQuestValues[Progress, FillGaps] = 0;
                }
            }

            if (SecondaryQuest == "None") SecondaryQuest = TempQuestStrings[0];
            PT2Strings = TempQuestStrings;
            PT2Values = TempQuestValues;
            PT2Subfolders = TempQuestSubfolders;
        }
        private void Locations(BinaryReader DJsIO, int NumOfLocations)
        {
            string[] TempLocationStrings = new string[NumOfLocations];

            for (int Progress = 0; Progress < NumOfLocations; Progress++)
                TempLocationStrings[Progress] = ReadString(DJsIO);
            LocationStrings = TempLocationStrings;
        }
        private void Echos(BinaryReader DJsIO, int NumOfEchos)
        {
            string[] TempEchoStrings = new string[NumOfEchos];
            int[,] TempEchoValues = new int[NumOfEchos, 2];

            for (int Progress = 0; Progress < NumOfEchos; Progress++)
            {
                TempEchoStrings[Progress] = ReadString(DJsIO);

                for (int TotalValues = 0; TotalValues < 2; TotalValues++)
                    TempEchoValues[Progress, TotalValues] = ReadInt32(DJsIO, EndianWSG);
            }
            EchoStrings = TempEchoStrings;
            EchoValues = TempEchoValues;
        }
        private void EchosPT2(BinaryReader DJsIO, int NumOfEchos)
        {
            string[] TempEchoStrings = new string[NumOfEchos];
            int[,] TempEchoValues = new int[NumOfEchos, 2];

            for (int Progress = 0; Progress < NumOfEchos; Progress++)
            {
                TempEchoStrings[Progress] = ReadString(DJsIO);

                for (int TotalValues = 0; TotalValues < 2; TotalValues++)
                    TempEchoValues[Progress, TotalValues] = ReadInt32(DJsIO, EndianWSG);
            }
            EchoStringsPT2 = TempEchoStrings;
            EchoValuesPT2 = TempEchoValues;
        }

        ///<summary>Reads a string in the format used by the WSG format</summary>
        public string ReadString(BinaryReader DJsIO)
        {
            int TempLengthValue = ReadInt32(DJsIO, EndianWSG);

            char[] TempOutput = new char[TempLengthValue - 1];
            byte[] bytes = DJsIO.ReadBytes(TempLengthValue - 1);

            for (int i = 0; i < TempLengthValue - 1; i++)
                TempOutput[i] = (Char)bytes[i];
            DJsIO.ReadByte();
            return new string(TempOutput);
        }
        ///<summary>Writes a string in the format used by the WSG format</summary>
        public byte[] WriteString(string InputString, ByteOrder Endian)
        {
            byte[] outBytes;
            if (InputString == "")
            {
                outBytes = new Byte[] { 0, 0, 0, 0 };
                return outBytes;
            }

            MemoryStream stream = new MemoryStream();
            BinaryWriter br = new BinaryWriter(stream);

            Write(br, InputString.Length + 1, Endian);
            foreach (char ch in InputString)
                br.Write((byte)ch);
            br.Write((byte)0);

            return stream.ToArray();
        }

        ///<summary>Save the current data to a WSG as a byte[]</summary>
        public byte[] SaveWSG()
        {
            MemoryStream OutStream = new MemoryStream();
            BinaryWriter Out = new BinaryWriter(OutStream);

            SplitInventoryIntoPacks();

            Write(Out, new System.Text.ASCIIEncoding().GetBytes(MagicHeader), EndianWSG);
            Write(Out, VersionNumber, EndianWSG);
            Write(Out, new System.Text.ASCIIEncoding().GetBytes(PLYR), EndianWSG);
            Write(Out, RevisionNumber, EndianWSG);
            Write(Out, WriteString(Class, EndianWSG), EndianWSG);
            Write(Out, Level, EndianWSG);
            Write(Out, Experience, EndianWSG);
            Write(Out, SkillPoints, EndianWSG);
            Write(Out, Unknown1, EndianWSG);
            Write(Out, Cash, EndianWSG);
            Write(Out, FinishedPlaythrough1, EndianWSG);
            Write(Out, NumberOfSkills, EndianWSG);

            for (int Progress = 0; Progress < NumberOfSkills; Progress++) //Write Skills
            {
                Write(Out, WriteString(SkillNames[Progress], EndianWSG), EndianWSG);
                Write(Out, LevelOfSkills[Progress], EndianWSG);
                Write(Out, ExpOfSkills[Progress], EndianWSG);
                Write(Out, InUse[Progress], EndianWSG);
            }

            Write(Out, Vehi1Color, EndianWSG);
            Write(Out, Vehi2Color, EndianWSG);
            Write(Out, Vehi1Type, EndianWSG);
            Write(Out, Vehi2Type, EndianWSG);
            Write(Out, NumberOfPools, EndianWSG);

            for (int Progress = 0; Progress < NumberOfPools; Progress++) //Write Ammo Pools
            {
                Write(Out, WriteString(ResourcePools[Progress], EndianWSG), EndianWSG);
                Write(Out, WriteString(AmmoPools[Progress], EndianWSG), EndianWSG);
                Write(Out, BitConverter.GetBytes((float)RemainingPools[Progress]), EndianWSG);
                Write(Out, PoolLevels[Progress], EndianWSG);
            }

            Write(Out, ItemStrings1.Count, EndianWSG);
            for (int Progress = 0; Progress < ItemStrings1.Count; Progress++) //Write Items
            {
                for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                    Write(Out, WriteString(ItemStrings1[Progress][TotalStrings], EndianWSG), EndianWSG);

                Write(Out, ItemValues1[Progress][0], EndianWSG);
                Int32 tempLevelQuality = ItemValues1[Progress][1] + ItemValues1[Progress][3] * 65536;
                Write(Out, tempLevelQuality, EndianWSG);
                Write(Out, ItemValues1[Progress][2], EndianWSG);
            }

            Write(Out, BackpackSize, EndianWSG);
            Write(Out, EquipSlots, EndianWSG);

            Write(Out, WeaponStrings1.Count, EndianWSG);
            for (int Progress = 0; Progress < WeaponStrings1.Count; Progress++) //Write Weapons
            {
                for (int TotalStrings1 = 0; TotalStrings1 < 14; TotalStrings1++)
                    Write(Out, WriteString(WeaponStrings1[Progress][TotalStrings1], EndianWSG), EndianWSG);

                Write(Out, WeaponValues1[Progress][0], EndianWSG);
                Int32 tempLevelQuality = WeaponValues1[Progress][1] + WeaponValues1[Progress][3] * 65536;
                Write(Out, tempLevelQuality, EndianWSG);
                Write(Out, WeaponValues1[Progress][2], EndianWSG);
            }

            Int16 count = (Int16)Challenges.Count();
            Write(Out, count * 7 + 10, EndianWSG);
            Write(Out, ChallengeDataBlockId, EndianWSG);
            Write(Out, count * 7 + 2, EndianWSG);
            WriteInt16(count, Out, EndianWSG);
            foreach (ChallengeDataEntry challenge in Challenges)
            {
                WriteInt16(challenge.Id, Out, EndianWSG);
                Out.Write(challenge.TypeId);
                Write(Out, challenge.Value, EndianWSG);
            }

            Write(Out, TotalLocations, EndianWSG);

            for (int Progress = 0; Progress < TotalLocations; Progress++) //Write Locations
                Write(Out, WriteString(LocationStrings[Progress], EndianWSG), EndianWSG);

            Write(Out, WriteString(CurrentLocation, EndianWSG), EndianWSG);
            Write(Out, SaveInfo1to5[0], EndianWSG);
            Write(Out, SaveInfo1to5[1], EndianWSG);
            Write(Out, SaveInfo1to5[2], EndianWSG);
            Write(Out, SaveInfo1to5[3], EndianWSG);
            Write(Out, SaveInfo1to5[4], EndianWSG);
            Write(Out, SaveNumber, EndianWSG);
            Write(Out, SaveInfo7to10[0], EndianWSG);
            Write(Out, SaveInfo7to10[1], EndianWSG);
            Write(Out, SaveInfo7to10[2], EndianWSG);
            Write(Out, SaveInfo7to10[3], EndianWSG);

            Write(Out, WriteString(CurrentQuest, EndianWSG), EndianWSG);
            Write(Out, TotalPT1Quests, EndianWSG);

            for (int Progress = 0; Progress < TotalPT1Quests; Progress++)  //Write Playthrough 1 Quests
            {
                Write(Out, WriteString(PT1Strings[Progress], EndianWSG), EndianWSG);
                for (int TotalValues = 0; TotalValues < 4; TotalValues++)
                    Write(Out, PT1Values[Progress, TotalValues], EndianWSG);

                if (PT1Values[Progress, 3] > 0) //Checks for subfolders
                {
                    for (int ExtraValues = 0; ExtraValues < PT1Values[Progress, 3]; ExtraValues++)
                    {
                        Write(Out, WriteString(PT1Subfolders[Progress, ExtraValues], EndianWSG), EndianWSG); //Write Subfolder
                        Write(Out, PT1Values[Progress, ExtraValues + 4], EndianWSG); //Write Subfolder value
                    }
                }
            }
            Write(Out, UnknownPT1QuestValue, EndianWSG);

            Write(Out, WriteString(SecondaryQuest, EndianWSG), EndianWSG);
            Write(Out, TotalPT2Quests, EndianWSG);

            for (int Progress = 0; Progress < TotalPT2Quests; Progress++)  //Write Playthrough 2 Quests
            {
                Write(Out, WriteString(PT2Strings[Progress], EndianWSG), EndianWSG);

                for (int TotalValues = 0; TotalValues < 4; TotalValues++)
                    Write(Out, PT2Values[Progress, TotalValues], EndianWSG);

                if (PT2Values[Progress, 3] > 0) //Checks for subfolders
                {
                    for (int ExtraValues = 0; ExtraValues < PT2Values[Progress, 3]; ExtraValues++)
                    {
                        Write(Out, WriteString(PT2Subfolders[Progress, ExtraValues], EndianWSG), EndianWSG); //Write Subfolder
                        Write(Out, PT2Values[Progress, ExtraValues + 4], EndianWSG); //Write Subfolder value

                    }
                }
            }
            Write(Out, UnknownPT2QuestValue, EndianWSG); //Is either 2 or 0

            Write(Out, WriteString("Z0_Missions.Missions.M_IntroStateSaver", EndianWSG), EndianWSG);
            Write(Out, 1, EndianWSG);
            Write(Out, WriteString("Z0_Missions.Missions.M_IntroStateSaver", EndianWSG), EndianWSG);
            Write(Out, 2, EndianWSG);
            Write(Out, 0, EndianWSG);
            Write(Out, 0, EndianWSG);
            Write(Out, 0, EndianWSG);
            Write(Out, TotalPlayTime, EndianWSG);
            Write(Out, WriteString(LastPlayedDate, EndianWSG), EndianWSG);
            Write(Out, WriteString(CharacterName, EndianWSG), EndianWSG);
            Write(Out, Color1, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Write(Out, Color2, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Write(Out, Color3, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Write(Out, Unknown2, EndianWSG);
            int NumberOfPromoCodesUsed = PromoCodesUsed.Count;
            Write(Out, NumberOfPromoCodesUsed, EndianWSG);
            for (int i = 0; i < NumberOfPromoCodesUsed; i++)
                Write(Out, PromoCodesUsed[i], EndianWSG);
            int NumberOfPromoCodesRequiringNotification = PromoCodesRequiringNotification.Count;
            Write(Out, NumberOfPromoCodesRequiringNotification, EndianWSG);
            for (int i = 0; i < NumberOfPromoCodesRequiringNotification; i++)
                Write(Out, PromoCodesRequiringNotification[i], EndianWSG);
            // Override the number of echo lists if the user edited the echo list and added PT2 echoes
            if (NumberOfEchosPT2 > 0)
                NumberOfEchoLists = 2;
            Write(Out, NumberOfEchoLists, EndianWSG);

            Write(Out, EchoIndex0, EndianWSG);
            Write(Out, NumberOfEchos, EndianWSG);
            for (int Progress = 0; Progress < NumberOfEchos; Progress++) //Write Locations
            {
                Write(Out, WriteString(EchoStrings[Progress], EndianWSG), EndianWSG);
                Write(Out, EchoValues[Progress, 0], EndianWSG);
                Write(Out, EchoValues[Progress, 1], EndianWSG);
            }

            if (NumberOfEchoLists > 1)
            {
                Write(Out, EchoIndex1, EndianWSG);
                Write(Out, NumberOfEchosPT2, EndianWSG);
                for (int Progress = 0; Progress < NumberOfEchosPT2; Progress++) //Write Locations
                {
                    Write(Out, WriteString(EchoStringsPT2[Progress], EndianWSG), EndianWSG);
                    Write(Out, EchoValuesPT2[Progress, 0], EndianWSG);
                    Write(Out, EchoValuesPT2[Progress, 1], EndianWSG);
                }
            }

            DLC.DLC_Size = 0;
            // This loop writes the base data for each section into byte[]
            // BaseData so its size can be obtained and it can easily be
            // written to the output stream as a single block.  Calculate
            // DLC.DLC_Size as it goes since that has to be written before
            // the blocks are written to the output stream.
            foreach (DLCSection Section in DLC.DataSections)
            {
                MemoryStream tempStream = new MemoryStream();
                BinaryWriter memwriter = new BinaryWriter(tempStream);
                switch (Section.Id)
                {
                    case DLC_Data.Section1Id:
                        memwriter.Write(DLC.DLC_Unknown1);
                        Write(memwriter, DLC.BankSize, EndianWSG);
                        // The bank item data is in RawData so it doesn't go here 
                        break;
                    case DLC_Data.Section2Id:
                        Write(memwriter, DLC.DLC_Unknown2, EndianWSG);
                        Write(memwriter, DLC.DLC_Unknown3, EndianWSG);
                        Write(memwriter, DLC.DLC_Unknown4, EndianWSG);
                        Write(memwriter, DLC.SkipDLC2Intro, EndianWSG);
                        break;
                    case DLC_Data.Section3Id:
                        memwriter.Write(DLC.DLC_Unknown5);
                        break;
                    case DLC_Data.Section4Id:
                        memwriter.Write(DLC.SecondaryPackEnabled);
                        // The DLC backpack items
                        Write(memwriter, ItemStrings2.Count, EndianWSG);
                        for (int Progress = 0; Progress < ItemStrings2.Count; Progress++) //Write Items
                        {
                            for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                                Write(memwriter, WriteString(ItemStrings2[Progress][TotalStrings], EndianWSG), EndianWSG);

                            Write(memwriter, ItemValues2[Progress][0], EndianWSG);
                            Int32 tempLevelQuality = ItemValues2[Progress][1] + ItemValues2[Progress][3] * 65536;
                            Write(memwriter, tempLevelQuality, EndianWSG);
                            Write(memwriter, ItemValues2[Progress][2], EndianWSG);
                        }
                        // The DLC backpack weapons
                        Write(memwriter, WeaponStrings2.Count, EndianWSG);
                        for (int Progress = 0; Progress < WeaponStrings2.Count; Progress++) //Write DLC.Weapons
                        {
                            for (int TotalStrings = 0; TotalStrings < 14; TotalStrings++)
                                Write(memwriter, WriteString(WeaponStrings2[Progress][TotalStrings], EndianWSG), EndianWSG);

                            Write(memwriter, WeaponValues2[Progress][0], EndianWSG);
                            Int32 tempLevelQuality = WeaponValues2[Progress][1] + WeaponValues2[Progress][3] * 65536;
                            Write(memwriter, tempLevelQuality, EndianWSG);
                            Write(memwriter, WeaponValues2[Progress][2], EndianWSG);
                        }
                        break;
                    default:
                        break;
                }
                Section.BaseData = tempStream.ToArray();
                DLC.DLC_Size += Section.BaseData.Count() + Section.RawData.Count() + 8; // 8 = 4 bytes for id, 4 bytes for length
            }

            // Now its time to actually write all the data sections to the output stream
            Write(Out, DLC.DLC_Size, EndianWSG);
            foreach (DLCSection Section in DLC.DataSections)
            {
                Write(Out, Section.Id, EndianWSG);
                int SectionLength = Section.BaseData.Count() + Section.RawData.Count();
                Write(Out, SectionLength, EndianWSG);
                Out.Write(Section.BaseData);
                Out.Write(Section.RawData);
                Section.BaseData = null; // BaseData isn't needed anymore.  Free it.
            }

            // Clear the temporary lists used to split primary and DLC pack data
            ItemValues1 = null;
            ItemStrings1 = null;
            ItemValues2 = null;
            ItemValues2 = null;
            WeaponValues1 = null;
            WeaponStrings1 = null;
            WeaponValues2 = null;
            WeaponValues2 = null;
            return OutStream.ToArray();
        }
        ///<summary>Split the weapon and item lists into two parts: one for the primary pack and one for DLC backpack</summary>
        public void SplitInventoryIntoPacks()
        {
            // Split items and weapons into two lists each so they can be put into the 
            // DLC backpack or regular backpack area as needed.  Any item with a level 
            // override and special dlc items go in the DLC backpack.  All others go 
            // in the regular inventory.
            if ((DLC.HasSection4 == false) || (DLC.SecondaryPackEnabled == 0))
            {
                // no secondary pack so put it all in primary pack
                ItemStrings1 = ItemStrings;
                ItemValues1 = ItemValues;
                WeaponStrings1 = WeaponStrings;
                WeaponValues1 = WeaponValues;
                ItemStrings2 = new List<List<string>>();
                ItemValues2 = new List<List<int>>();
                WeaponStrings2 = new List<List<string>>();
                WeaponValues2 = new List<List<int>>();
                return;
            }

            ItemStrings1 = new List<List<string>>();
            ItemValues1 = new List<List<int>>();
            ItemStrings2 = new List<List<string>>();
            ItemValues2 = new List<List<int>>();

            int ItemCount = ItemStrings.Count;
            for (int i = 0; i < ItemCount; i++)
            {
                if ((ItemValues[i][3] == 0) && (ItemStrings[i][0].Substring(0, 3) != "dlc"))
                {
                    ItemStrings1.Add(ItemStrings[i]);
                    ItemValues1.Add(ItemValues[i]);
                }
                else
                {
                    ItemStrings2.Add(ItemStrings[i]);
                    ItemValues2.Add(ItemValues[i]);
                }
            }

            WeaponStrings1 = new List<List<string>>();
            WeaponValues1 = new List<List<int>>();
            WeaponStrings2 = new List<List<string>>();
            WeaponValues2 = new List<List<int>>();

            int WeaponCount = WeaponStrings.Count;
            for (int i = 0; i < WeaponCount; i++)
            {
                if ((WeaponValues[i][3] == 0) && (WeaponStrings[i][0].Substring(0, 3) != "dlc"))
                {
                    WeaponStrings1.Add(WeaponStrings[i]);
                    WeaponValues1.Add(WeaponValues[i]);
                }
                else
                {
                    WeaponStrings2.Add(WeaponStrings[i]);
                    WeaponValues2.Add(WeaponValues[i]);
                }
            }
        }
    }
}