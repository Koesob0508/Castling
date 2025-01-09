using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager
{
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationId;
        public byte[] AllocationIdBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    public struct RelayJoinData
    {
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationId;
        public byte[] AllocationIdBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

    public static async Task<RelayHostData> SetupRelay(int maxConnections, string environment)
    {
        await InitializeServicesIfNeeded(environment);
        await EnsureSignedInAsync();

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        RelayHostData data = new RelayHostData
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId),
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationId = allocation.AllocationId,
            AllocationIdBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key
        };

        return data;
    }

    public static async Task<RelayJoinData> JoinRelay(string joinCode, string environment)
    {
        await InitializeServicesIfNeeded(environment);
        await EnsureSignedInAsync();

        if (!IsValidJoinCode(joinCode))
        {
            Debug.LogError($"Invalid Join Code: {joinCode}");
            return default;
        }

        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"RelayServiceException during JoinRelay: {e.Message}");
            return default;
        }

        return new RelayJoinData
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationId = allocation.AllocationId,
            AllocationIdBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key
        };
    }

    private static async Task InitializeServicesIfNeeded(string environment)
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);
        }
    }

    private static async Task EnsureSignedInAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private static bool IsValidJoinCode(string joinCode)
    {
        string pattern = "^[6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw]{6,12}$";
        return !string.IsNullOrEmpty(joinCode) && System.Text.RegularExpressions.Regex.IsMatch(joinCode, pattern);
    }

    public static string CleanJoinCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        // 보이지 않는 특수 문자 및 제어 문자를 제거합니다.
        return code.Trim().Replace("\u200B", "").Replace("\uFEFF", "").Replace(" ", "");
    }
}