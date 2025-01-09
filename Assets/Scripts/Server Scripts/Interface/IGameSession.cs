using System.Collections.Generic;

namespace Castling.Server
{
    public interface IGameSession
    {
        int SessionID { get; }
        void Init(int sessionID, List<PlayerInfo> playerInfos);
        void Clear();
    }
}