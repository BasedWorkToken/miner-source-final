/*
   Copyright 2018 Lip Wee Yeo Amano

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using System.Threading.Tasks;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace SoliditySHA3Miner.NetworkInterface
{
    public class MiningParameters
    {
        public HexBigInteger MiningDifficulty { get; private set; }
        public HexBigInteger MiningDifficulty2 { get; private set; }
        public HexBigInteger MiningTarget { get; private set; }
        public HexBigInteger Challenge { get; private set; }
        public HexBigInteger MaximumTarget { get; private set; }
        public byte[] MiningTargetByte32 { get; private set; }
        public byte[] ChallengeByte32 { get; private set; }
        public byte[] MaximumTargetByte32 { get; private set; }
        public byte[] EthAddressByte20 { get; private set; }
        public byte[] KingAddressByte20 { get; private set; }
        public string EthAddress { get; private set; }
        public string MiningTargetByte32String { get; private set; }
        public string MaximumTargetByte32String { get; private set; }
        public string ChallengeByte32String { get; private set; }
        public string KingAddress { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsPoolMining { get; private set; }

        public static MiningParameters GetSoloMiningParameters(string ethAddress,
                                                               Function getMiningDifficulty,
                                                                Function getMiningDifficulty2,
                                                               Function getMiningTarget,
                                                               Function getChallengeNumber, int multiplier)
        {
            return new MiningParameters(ethAddress, getMiningDifficulty, getMiningDifficulty2, getMiningTarget, getChallengeNumber, multiplier);
        }

        public static MiningParameters GetMiningParameters(string masterURL,
                                                           JObject getEthAddress = null,
                                                           JObject getChallenge = null,
                                                           JObject getDifficulty = null,
                                                           JObject getTarget = null,
                                                           JObject getMaximumTarget = null,
                                                           JObject getKingAddress = null,
                                                           JObject getPause = null,
                                                           JObject getPoolMining = null)
        {
            return new MiningParameters(masterURL,
                                        getEthAddress: getEthAddress,
                                        getChallenge: getChallenge,
                                        getDifficulty: getDifficulty,
                                        getTarget: getTarget,
                                        getMaximumTarget: getMaximumTarget,
                                        getKingAddress: getKingAddress,
                                        getPause: getPause,
                                        getPoolMining: getPoolMining);
        }

        private MiningParameters(string ethAddress,
                                 Function getMiningDifficulty,
                                 Function getMiningDifficulty2,
                                 Function getMiningTarget,
                                 Function getChallengeNumber, int multiplier)
        {
            EthAddress = ethAddress;
            var ethAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
            Utils.Numerics.AddressStringToByte20Array(EthAddress, ref ethAddressByte20);
            EthAddressByte20 = ethAddressByte20;

            var retryCount = 0;

            while (retryCount < 10)
                try
                {
                    MiningDifficulty = new HexBigInteger(getMiningDifficulty.CallAsync<BigInteger>().Result * BigInteger.Pow(2, multiplier));

                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                } 
            while (retryCount < 10)
                try
                {
                    MiningDifficulty2 = new HexBigInteger(getMiningDifficulty2.CallAsync<BigInteger>().Result * BigInteger.Pow(2, multiplier));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    var testzsfdsdfsdf = getMiningTarget.CallAsync<BigInteger>().Result;
                    Program.Print(string.Format("[FFFFFFFFFINFO34] New Target testzsfdsdfsdf ({0})...", testzsfdsdfsdf));

                    var divideby = BigInteger.Pow(2, multiplier);
                    Program.Print(string.Format("[FFFFFFFFFINFO34] New divideby divideby ({0})...", divideby));

                    // Perform division
                    var dividedTarget = BigInteger.Divide(testzsfdsdfsdf, divideby);

                    // Optional: Additional diagnostics
                    Program.Print($"Divide by: {divideby}");
                    Program.Print($"Divided Target: {dividedTarget}");
                    Program.Print($"Divided Target Hex: {dividedTarget.ToString("X")}");

                    MiningTarget = new HexBigInteger(dividedTarget);
                    //MiningTarget = new HexBigInteger(testzsfdsdfsdf >> multiplier);
                    Program.Print(string.Format("[FFFFFFFFFINFO34] multipliermultipliermultipliermultipliermultipliermultiplier({0})...", multiplier));
                    Program.Print(string.Format("[FFFFFFFFFINFO34] multipliermultipliermultipliermultipliermultipliermultiplier({0})...", multiplier));
                    Program.Print(string.Format("[FFFFFFFFFINFO34] multipliermultipliermultipliermultipliermultipliermultiplier({0})...", multiplier));
                    MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                    Program.Print(string.Format("[FFFFFFFFFINFO34] New Target ({0})...", MiningTarget.Value.ToString("X")));


                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(getChallengeNumber.CallAsync<byte[]>().Result);
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    Challenge = new HexBigInteger(new BigInteger(ChallengeByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge from network: " + ex.Message, ex.InnerException);
                }
        }

        private MiningParameters(string url,
                                 JObject getEthAddress = null,
                                 JObject getChallenge = null,
                                 JObject getDifficulty = null,
                                 JObject getDifficulty2 = null,
                                 JObject getTarget = null,
                                 JObject getMaximumTarget = null,
                                 JObject getKingAddress = null,
                                 JObject getPause = null,
                                 JObject getPoolMining = null)
        {
            var retryCount = 0;
            
            while (getEthAddress != null)
                try
                {
                    EthAddress = Utils.Json.InvokeJObjectRPC(url, getEthAddress).SelectToken("$.result").Value<string>();
                    var ethAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                    // some pools provide invalid checksum address
                    Utils.Numerics.AddressStringToByte20Array(EthAddress, ref ethAddressByte20, isChecksum:false);
                    EthAddressByte20 = ethAddressByte20;
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("1Failed to get address: " + ex.Message, ex.InnerException);
                }

            while (getChallenge != null)
                try
                {
                    Challenge = new HexBigInteger(Utils.Json.InvokeJObjectRPC(url, getChallenge).SelectToken("$.result").Value<string>());
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(Challenge.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge: " + ex.Message, ex.InnerException);
                }

            while (getDifficulty != null)
                try
                {
                    var rawDifficulty = Utils.Json.InvokeJObjectRPC(url, getDifficulty).SelectToken("$.result").Value<string>();
                    if (rawDifficulty.StartsWith("0x"))
                        MiningDifficulty = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty), isUnsigned: true, isBigEndian: true)*100);
                    else
                        MiningDifficulty = new HexBigInteger(BigInteger.Parse(rawDifficulty) * 100);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty: " + ex.Message, ex.InnerException);
                }


            while (getDifficulty2 != null)
                try
                {
                    var rawDifficulty2= Utils.Json.InvokeJObjectRPC(url, getDifficulty2).SelectToken("$.result").Value<string>();

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", rawDifficulty2));
                    if (rawDifficulty2.StartsWith("0x"))
                        MiningDifficulty2 = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty2), isUnsigned: true, isBigEndian: true) * 100);
                    else
                        MiningDifficulty2 = new HexBigInteger(BigInteger.Parse(rawDifficulty2) * 100);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty NUMBER 2: " + ex.Message, ex.InnerException);
                }

            while (getTarget != null)
                try
                {
                    var rawMiningTarget = Utils.Json.InvokeJObjectRPC(url, getTarget).SelectToken("$.result").Value<string>();
                    if (rawMiningTarget.StartsWith("0x"))
                    {
                        MiningTargetByte32String = rawMiningTarget;
                        MiningTargetByte32 = Utils.Numerics.HexStringToByte32Array(rawMiningTarget);
                        MiningTarget = new HexBigInteger(new BigInteger(MiningTargetByte32, isUnsigned: true, isBigEndian: true));
                        Program.Print(string.Format("[FFFFFFFFFINFO] New Target ({0})...", MiningTarget));
                    }
                    else // currently pool returns in decimal format
                    {
                        MiningTarget = new HexBigInteger(BigInteger.Parse(rawMiningTarget));
                        MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                        MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                        Program.Print(string.Format("[FFFFFFFFFINFO2] New Target ({0})...", MiningTarget));
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target: " + ex.Message, ex.InnerException);
                }

            while (getMaximumTarget != null)
                try
                {
                    MaximumTargetByte32String = Utils.Json.InvokeJObjectRPC(url, getMaximumTarget).SelectToken("$.result").Value<string>();
                    MaximumTargetByte32 = Utils.Numerics.HexStringToByte32Array(MaximumTargetByte32String);
                    MaximumTarget = new HexBigInteger(new BigInteger(MaximumTargetByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("123Failed to get maximum target: " + ex.Message, ex.InnerException);
                }

            while (getKingAddress != null)
                try
                {
                    KingAddress = Utils.Json.InvokeJObjectRPC(url, getKingAddress).SelectToken("$.result").Value<string>();

                    if (!string.IsNullOrWhiteSpace(KingAddress))
                    {
                        var kingAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                        Utils.Numerics.AddressStringToByte20Array(KingAddress, ref kingAddressByte20, "king address");
                        KingAddressByte20 = kingAddressByte20;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get king address: " + ex.Message, ex.InnerException);
                }

            while (getPause != null)
                try
                {
                    var pauseString = Utils.Json.InvokeJObjectRPC(url, getPause).SelectToken("$.result").Value<string>();
                    IsPause = bool.Parse(pauseString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pause: " + ex.Message, ex.InnerException);
                }

            while (getPoolMining != null)
                try
                {
                    var poolMiningString = Utils.Json.InvokeJObjectRPC(url, getPoolMining).SelectToken("$.result").Value<string>();
                    IsPoolMining = bool.Parse(poolMiningString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pool mining: " + ex.Message, ex.InnerException);
                }
        }
    }
    public class MiningParameters2
    {
        public HexBigInteger MiningDifficulty { get; private set; }
        public HexBigInteger MiningDifficulty2 { get; private set; }
        public HexBigInteger MiningTarget { get; private set; }
        public HexBigInteger Challenge { get; private set; }
        public HexBigInteger MaximumTarget { get; private set; }
        public byte[] MiningTargetByte32 { get; private set; }
        public byte[] ChallengeByte32 { get; private set; }
        public byte[] MaximumTargetByte32 { get; private set; }
        public byte[] EthAddressByte20 { get; private set; }
        public byte[] KingAddressByte20 { get; private set; }
        public string EthAddress { get; private set; }
        public string MiningTargetByte32String { get; private set; }
        public string MaximumTargetByte32String { get; private set; }
        public string ChallengeByte32String { get; private set; }
        public string KingAddress { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsPoolMining { get; private set; }

        public static MiningParameters2 GetSoloMiningParameters2(string ethAddress,
                                                                Function getMiningDifficulty2,
                                                               Function getChallengeNumber, int multiz)
        {
            return new MiningParameters2(ethAddress, getMiningDifficulty2, getChallengeNumber, multiz);
        }

        public static MiningParameters2 GetMiningParameters2(string masterURL,
                                                           JObject getEthAddress = null,
                                                           JObject getChallenge = null,
                                                           JObject getDifficulty = null,
                                                           JObject getTarget = null,
                                                           JObject getMaximumTarget = null,
                                                           JObject getKingAddress = null,
                                                           JObject getPause = null,
                                                           JObject getPoolMining = null)
        {
            return new MiningParameters2(masterURL,
                                        getDifficulty: getDifficulty, getChallenge: getChallenge);
        }

        private MiningParameters2(string ethAddress,

                                 Function getMiningDifficulty2,
                                 Function getChallengeNumber, int multi
                                 )
        {
            EthAddress = ethAddress;
            var ethAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
            Utils.Numerics.AddressStringToByte20Array(EthAddress, ref ethAddressByte20);
            EthAddressByte20 = ethAddressByte20;

            var retryCount = 0;

            while (retryCount < 10)
                try
                {
                    //MiningDifficulty = new HexBigInteger(getMiningDifficulty.CallAsync<BigInteger>().Result);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }
            while (retryCount < 10)
                try
                {
                    MiningDifficulty2 = new HexBigInteger(getMiningDifficulty2.CallAsync<BigInteger>().Result * BigInteger.Pow(2, multi));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    // MiningTarget = new HexBigInteger(getMiningTarget.CallAsync<BigInteger>().Result);
                    //MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    //MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(getChallengeNumber.CallAsync<byte[]>().Result);
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    Challenge = new HexBigInteger(new BigInteger(ChallengeByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge from network: " + ex.Message, ex.InnerException);
                }
        }

        private MiningParameters2(string url,
                                 JObject getEthAddress = null,
                                 JObject getChallenge = null,
                                 JObject getDifficulty = null,
                                 JObject getDifficulty2 = null,
                                 JObject getTarget = null,
                                 JObject getMaximumTarget = null,
                                 JObject getKingAddress = null,
                                 JObject getPause = null,
                                 JObject getPoolMining = null)
        {
            var retryCount = 0;

            
            while (getChallenge != null)
                try
                {
                    Challenge = new HexBigInteger(Utils.Json.InvokeJObjectRPC(url, getChallenge).SelectToken("$.result").Value<string>());
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(Challenge.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge: " + ex.Message, ex.InnerException);
                }

            while (getDifficulty != null)
                try
                {
                    var rawDifficulty = Utils.Json.InvokeJObjectRPC(url, getDifficulty).SelectToken("$.result").Value<string>();
                    if (rawDifficulty.StartsWith("0x"))
                        MiningDifficulty = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty), isUnsigned: true, isBigEndian: true));
                    else
                        MiningDifficulty = new HexBigInteger(BigInteger.Parse(rawDifficulty));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty: " + ex.Message, ex.InnerException);
                }


            while (getDifficulty2 != null)
                try
                {
                    var rawDifficulty2 = Utils.Json.InvokeJObjectRPC(url, getDifficulty2).SelectToken("$.result").Value<string>();

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", rawDifficulty2));
                    if (rawDifficulty2.StartsWith("0x"))
                        MiningDifficulty2 = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty2), isUnsigned: true, isBigEndian: true));
                    else
                        MiningDifficulty2 = new HexBigInteger(BigInteger.Parse(rawDifficulty2));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty NUMBER 2: " + ex.Message, ex.InnerException);
                }


            while (getMaximumTarget != null)
                try
                {
                    MaximumTargetByte32String = Utils.Json.InvokeJObjectRPC(url, getMaximumTarget).SelectToken("$.result").Value<string>();
                    MaximumTargetByte32 = Utils.Numerics.HexStringToByte32Array(MaximumTargetByte32String);
                    MaximumTarget = new HexBigInteger(new BigInteger(MaximumTargetByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("456Failed to get maximum target: " + ex.Message, ex.InnerException);
                }

            while (getKingAddress != null)
                try
                {
                    KingAddress = Utils.Json.InvokeJObjectRPC(url, getKingAddress).SelectToken("$.result").Value<string>();

                    if (!string.IsNullOrWhiteSpace(KingAddress))
                    {
                        var kingAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                        Utils.Numerics.AddressStringToByte20Array(KingAddress, ref kingAddressByte20, "king address");
                        KingAddressByte20 = kingAddressByte20;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get king address: " + ex.Message, ex.InnerException);
                }

            while (getPause != null)
                try
                {
                    var pauseString = Utils.Json.InvokeJObjectRPC(url, getPause).SelectToken("$.result").Value<string>();
                    IsPause = bool.Parse(pauseString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pause: " + ex.Message, ex.InnerException);
                }

            while (getPoolMining != null)
                try
                {
                    var poolMiningString = Utils.Json.InvokeJObjectRPC(url, getPoolMining).SelectToken("$.result").Value<string>();
                    IsPoolMining = bool.Parse(poolMiningString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pool mining: " + ex.Message, ex.InnerException);
                }
        }
    }

    public class MiningParameters3
    {
        public HexBigInteger MiningDifficulty { get; private set; }
        public HexBigInteger MiningDifficulty2 { get; private set; }
        public HexBigInteger MiningTarget { get; private set; }
        public HexBigInteger Challenge { get; private set; }
        public HexBigInteger MaximumTarget { get; private set; }
        public byte[] MiningTargetByte32 { get; private set; }
        public byte[] ChallengeByte32 { get; private set; }
        public byte[] MaximumTargetByte32 { get; private set; }
        public byte[] EthAddressByte20 { get; private set; }
        public byte[] KingAddressByte20 { get; private set; }
        public string EthAddress { get; private set; }
        public string MiningTargetByte32String { get; private set; }
        public string MaximumTargetByte32String { get; private set; }
        public string ChallengeByte32String { get; private set; }
        public string KingAddress { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsPoolMining { get; private set; }

        public static MiningParameters3 GetSoloMiningParameters3(string ethAddress,
                                                                Function getMiningDifficulty3,
                                                                Function getMiningDifficulty4,
                                                               Function getChallengeNumber)
        {
            return new MiningParameters3(ethAddress, getMiningDifficulty3, getMiningDifficulty4, getChallengeNumber);
        }

        public static MiningParameters3 GetMiningParameters3(string masterURL,
                                                           JObject getEthAddress = null,
                                                           JObject getChallenge = null,
                                                           JObject getDifficulty = null,
                                                           JObject getDifficulty2 = null,
                                                           JObject getTarget = null,
                                                           JObject getMaximumTarget = null,
                                                           JObject getKingAddress = null,
                                                           JObject getPause = null,
                                                           JObject getPoolMining = null)
        {
            return new MiningParameters3(masterURL,
                                        getDifficulty: getDifficulty, getDifficulty2: getDifficulty2, getChallenge: getChallenge);
        }

        private MiningParameters3(string ethAddress,

                                 Function getMiningDifficulty3,
                                 Function getMiningDifficulty4,
                                 Function getChallengeNumber
                                 )
        {
            
            var retryCount = 0;

            while (retryCount < 10)
                try
                {
                    MiningDifficulty = new HexBigInteger(getMiningDifficulty4.CallAsync<BigInteger>().Result);

                    //Program.Print(string.Format("[INFO] MiningDifficulty ({0})...", MiningDifficulty));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }
            while (retryCount < 10)
                try
                {
                    MiningDifficulty2 = new HexBigInteger(getMiningDifficulty3.CallAsync<BigInteger>().Result);

                    //Program.Print(string.Format("[INFO] MiningDifficulty2 ({0})...", MiningDifficulty));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    // MiningTarget = new HexBigInteger(getMiningTarget.CallAsync<BigInteger>().Result);
                    //MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    //MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(getChallengeNumber.CallAsync<byte[]>().Result);
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    Challenge = new HexBigInteger(new BigInteger(ChallengeByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge from network: " + ex.Message, ex.InnerException);
                }
        }

        private MiningParameters3(string url,
                                 JObject getEthAddress = null,
                                 JObject getChallenge = null,
                                 JObject getDifficulty = null,
                                 JObject getDifficulty2 = null,
                                 JObject getDifficulty3 = null,
                                 JObject getDifficulty4 = null,
                                 JObject getTarget = null,
                                 JObject getMaximumTarget = null,
                                 JObject getKingAddress = null,
                                 JObject getPause = null,
                                 JObject getPoolMining = null)
        {
            var retryCount = 0;

            while (getChallenge != null)
                try
                {
                    Challenge = new HexBigInteger(Utils.Json.InvokeJObjectRPC(url, getChallenge).SelectToken("$.result").Value<string>());
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(Challenge.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge: " + ex.Message, ex.InnerException);
                }

            while (getDifficulty4 != null)
                try
                {
                    var rawDifficulty = Utils.Json.InvokeJObjectRPC(url, getDifficulty4).SelectToken("$.result").Value<string>();
                    if (rawDifficulty.StartsWith("0x"))
                        MiningDifficulty = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty), isUnsigned: true, isBigEndian: true));
                    else
                        MiningDifficulty = new HexBigInteger(BigInteger.Parse(rawDifficulty));

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", MiningDifficulty));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty: " + ex.Message, ex.InnerException);
                }


            while (getDifficulty3 != null)
                try
                {
                    var rawDifficulty2 = Utils.Json.InvokeJObjectRPC(url, getDifficulty3).SelectToken("$.result").Value<string>();

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", rawDifficulty2));
                    if (rawDifficulty2.StartsWith("0x"))
                        MiningDifficulty2 = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty2), isUnsigned: true, isBigEndian: true));
                    else
                        MiningDifficulty2 = new HexBigInteger(BigInteger.Parse(rawDifficulty2));
                    break;

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", MiningDifficulty));
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty NUMBER 2: " + ex.Message, ex.InnerException);
                }

           
            while (getMaximumTarget != null)
                try
                {
                    MaximumTargetByte32String = Utils.Json.InvokeJObjectRPC(url, getMaximumTarget).SelectToken("$.result").Value<string>();
                    MaximumTargetByte32 = Utils.Numerics.HexStringToByte32Array(MaximumTargetByte32String);
                    MaximumTarget = new HexBigInteger(new BigInteger(MaximumTargetByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("678Failed to get maximum target: " + ex.Message, ex.InnerException);
                }

            while (getKingAddress != null)
                try
                {
                    KingAddress = Utils.Json.InvokeJObjectRPC(url, getKingAddress).SelectToken("$.result").Value<string>();

                    if (!string.IsNullOrWhiteSpace(KingAddress))
                    {
                        var kingAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                        Utils.Numerics.AddressStringToByte20Array(KingAddress, ref kingAddressByte20, "king address");
                        KingAddressByte20 = kingAddressByte20;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get king address: " + ex.Message, ex.InnerException);
                }

            while (getPause != null)
                try
                {
                    var pauseString = Utils.Json.InvokeJObjectRPC(url, getPause).SelectToken("$.result").Value<string>();
                    IsPause = bool.Parse(pauseString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pause: " + ex.Message, ex.InnerException);
                }

            while (getPoolMining != null)
                try
                {
                    var poolMiningString = Utils.Json.InvokeJObjectRPC(url, getPoolMining).SelectToken("$.result").Value<string>();
                    IsPoolMining = bool.Parse(poolMiningString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pool mining: " + ex.Message, ex.InnerException);
                }
        }
    }








































    public class MiningParameters4
    {
        public HexBigInteger Epoch { get; private set; }
        public HexBigInteger MiningDifficulty2 { get; private set; }
        public HexBigInteger MiningTarget { get; private set; }
        public HexBigInteger Challenge { get; private set; }
        public HexBigInteger MaximumTarget { get; private set; }
        public byte[] MiningTargetByte32 { get; private set; }
        public byte[] ChallengeByte32 { get; private set; }
        public byte[] MaximumTargetByte32 { get; private set; }
        public byte[] EthAddressByte20 { get; private set; }
        public byte[] KingAddressByte20 { get; private set; }
        public string EthAddress { get; private set; }
        public string MiningTargetByte32String { get; private set; }
        public string MaximumTargetByte32String { get; private set; }
        public string ChallengeByte32String { get; private set; }
        public string KingAddress { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsPoolMining { get; private set; }

        public static MiningParameters4 GetSoloMiningParameters4(string ethAddress,
                                                                Function getEpoch,
                                                                Function getMiningDifficulty4,
                                                               Function getChallengeNumber)
        {
            return new MiningParameters4(ethAddress, getEpoch, getMiningDifficulty4, getChallengeNumber);
        }

        public static MiningParameters4 GetMiningParameters4(string masterURL,
                                                           JObject getEthAddress = null,
                                                           JObject getChallenge = null,
                                                           JObject getEpoch = null,
                                                           JObject getDifficulty2 = null,
                                                           JObject getTarget = null,
                                                           JObject getMaximumTarget = null,
                                                           JObject getKingAddress = null,
                                                           JObject getPause = null,
                                                           JObject getPoolMining = null)
        {
            return new MiningParameters4(masterURL,
                                        getEpoch: getEpoch, getDifficulty2: getDifficulty2, getChallenge: getChallenge);
        }

        private MiningParameters4(string ethAddress,

                                 Function getEpoch,
                                 Function getMiningDifficulty4,
                                 Function getChallengeNumber
                                 )
        {
            
            var retryCount = 0;

            while (retryCount < 10)
                try
                {
                    Epoch = new HexBigInteger(getEpoch.CallAsync<BigInteger>().Result);

                    //Program.Print(string.Format("[INFO] EPOCH ({0})...", Epoch));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("4Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }
            while (retryCount < 10)
                try
                {

                    //Program.Print(string.Format("[INFO] MiningDifficulty2 ({0})...", getMiningDifficulty4.ToString()));
                    MiningDifficulty2 = new HexBigInteger(getMiningDifficulty4.CallAsync<BigInteger>().Result);

                    //Program.Print(string.Format("[INFO] MiningDifficulty2 ({0})...", MiningDifficulty2.Value));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty from network: " + ex.Message, ex.InnerException);
                }

            while (retryCount < 10)
                try
                {
                    // MiningTarget = new HexBigInteger(getMiningTarget.CallAsync<BigInteger>().Result);
                    //MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    //MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target from network: " + ex.Message, ex.InnerException);
                }
            /*
            while (retryCount < 10)
                try
                {
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(getChallengeNumber.CallAsync<byte[]>().Result);
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    Challenge = new HexBigInteger(new BigInteger(ChallengeByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge from network: " + ex.Message, ex.InnerException);
                }
            */
        }

        private MiningParameters4(string url,
                                 JObject getEthAddress = null,
                                 JObject getChallenge = null,
                                 JObject getEpoch = null,
                                 JObject getDifficulty2 = null,
                                 JObject getDifficulty3 = null,
                                 JObject getDifficulty4 = null,
                                 JObject getTarget = null,
                                 JObject getMaximumTarget = null,
                                 JObject getKingAddress = null,
                                 JObject getPause = null,
                                 JObject getPoolMining = null)
        {
            var retryCount = 0;
            /*
            while (getEthAddress != null)
                try
                {
                    EthAddress = Utils.Json.InvokeJObjectRPC(url, getEthAddress).SelectToken("$.result").Value<string>();
                    var ethAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                    // some pools provide invalid checksum address
                    Utils.Numerics.AddressStringToByte20Array(EthAddress, ref ethAddressByte20, isChecksum: false);
                    EthAddressByte20 = ethAddressByte20;
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get address: " + ex.Message, ex.InnerException);
                }

            while (getChallenge != null)
                try
                {
                    Challenge = new HexBigInteger(Utils.Json.InvokeJObjectRPC(url, getChallenge).SelectToken("$.result").Value<string>());
                    ChallengeByte32 = Utils.Numerics.FilterByte32Array(Challenge.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                    ChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(ChallengeByte32);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get challenge: " + ex.Message, ex.InnerException);
                }
            */
            while (Epoch != null)
                try
                {
                    var rawDifficulty = Utils.Json.InvokeJObjectRPC(url, getEpoch).SelectToken("$.result").Value<string>();
                    if (rawDifficulty.StartsWith("0x"))
                        Epoch = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty), isUnsigned: true, isBigEndian: true));
                    else
                        Epoch = new HexBigInteger(BigInteger.Parse(rawDifficulty));

                    Program.Print(string.Format("[INFO] New EPOCH detected2 ({0})...", Epoch));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty: " + ex.Message, ex.InnerException);
                }
            /*

            while (getDifficulty3 != null)
                try
                {
                    var rawDifficulty2 = Utils.Json.InvokeJObjectRPC(url, getDifficulty3).SelectToken("$.result").Value<string>();

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", rawDifficulty2));
                    if (rawDifficulty2.StartsWith("0x"))
                        MiningDifficulty2 = new HexBigInteger(new BigInteger(Utils.Numerics.HexStringToByte32Array(rawDifficulty2), isUnsigned: true, isBigEndian: true));
                    else
                        MiningDifficulty2 = new HexBigInteger(BigInteger.Parse(rawDifficulty2));
                    break;

                    Program.Print(string.Format("[INFO] New difficulty detected2 ({0})...", MiningDifficulty2));
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get difficulty NUMBER 2: " + ex.Message, ex.InnerException);
                }

            while (getTarget != null)
                try
                {
                    var rawMiningTarget = Utils.Json.InvokeJObjectRPC(url, getTarget).SelectToken("$.result").Value<string>();
                    if (rawMiningTarget.StartsWith("0x"))
                    {
                        MiningTargetByte32String = rawMiningTarget;
                        MiningTargetByte32 = Utils.Numerics.HexStringToByte32Array(rawMiningTarget);
                        MiningTarget = new HexBigInteger(new BigInteger(MiningTargetByte32, isUnsigned: true, isBigEndian: true));
                    }
                    else // currently pool returns in decimal format
                    {
                        MiningTarget = new HexBigInteger(BigInteger.Parse(rawMiningTarget));
                        MiningTargetByte32 = Utils.Numerics.FilterByte32Array(MiningTarget.Value.ToByteArray(isUnsigned: true, isBigEndian: true));
                        MiningTargetByte32String = Utils.Numerics.Byte32ArrayToHexString(MiningTargetByte32);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get target: " + ex.Message, ex.InnerException);
                }

            while (getMaximumTarget != null)
                try
                {
                    MaximumTargetByte32String = Utils.Json.InvokeJObjectRPC(url, getMaximumTarget).SelectToken("$.result").Value<string>();
                    MaximumTargetByte32 = Utils.Numerics.HexStringToByte32Array(MaximumTargetByte32String);
                    MaximumTarget = new HexBigInteger(new BigInteger(MaximumTargetByte32, isUnsigned: true, isBigEndian: true));
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get maximum target: " + ex.Message, ex.InnerException);
                }

            while (getKingAddress != null)
                try
                {
                    KingAddress = Utils.Json.InvokeJObjectRPC(url, getKingAddress).SelectToken("$.result").Value<string>();

                    if (!string.IsNullOrWhiteSpace(KingAddress))
                    {
                        var kingAddressByte20 = (byte[])Array.CreateInstance(typeof(byte), Miner.MinerBase.ADDRESS_LENGTH);
                        Utils.Numerics.AddressStringToByte20Array(KingAddress, ref kingAddressByte20, "king address");
                        KingAddressByte20 = kingAddressByte20;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get king address: " + ex.Message, ex.InnerException);
                }

            while (getPause != null)
                try
                {
                    var pauseString = Utils.Json.InvokeJObjectRPC(url, getPause).SelectToken("$.result").Value<string>();
                    IsPause = bool.Parse(pauseString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pause: " + ex.Message, ex.InnerException);
                }

            while (getPoolMining != null)
                try
                {
                    var poolMiningString = Utils.Json.InvokeJObjectRPC(url, getPoolMining).SelectToken("$.result").Value<string>();
                    IsPoolMining = bool.Parse(poolMiningString);
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < 10)
                        Task.Delay(500).Wait();
                    else
                        throw new OperationCanceledException("Failed to get pool mining: " + ex.Message, ex.InnerException);
                }

                */
        }
    }
}