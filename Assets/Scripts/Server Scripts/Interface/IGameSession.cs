using System.Collections.Generic;

namespace Castling.Server
{
    public interface IGameSession
    {
        public void Init(List<PlayerInfo> playerInfos);
    }
}