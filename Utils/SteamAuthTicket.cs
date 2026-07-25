using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SteamAuthOnOculus.Utils
{
    public class SteamAuthTicket : IDisposable // the rift build of gorilla tag doesnt have this, just copied and pasted it from dnspy
    {
        // Token: 0x0600504F RID: 20559 RVA: 0x001A8C6C File Offset: 0x001A6E6C
        private SteamAuthTicket(HAuthTicket hAuthTicket)
        {
            this.m_hAuthTicket = hAuthTicket;
        }

        // Token: 0x06005050 RID: 20560 RVA: 0x001A8C7B File Offset: 0x001A6E7B
        public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
        {
            return new SteamAuthTicket(hAuthTicket);
        }

        // Token: 0x06005051 RID: 20561 RVA: 0x001A8C84 File Offset: 0x001A6E84
        ~SteamAuthTicket()
        {
            this.Dispose();
        }

        // Token: 0x06005052 RID: 20562 RVA: 0x001A8CB0 File Offset: 0x001A6EB0
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.m_hAuthTicket != HAuthTicket.Invalid)
            {
                try
                {
                    SteamUser.CancelAuthTicket(this.m_hAuthTicket);
                }
                catch (InvalidOperationException)
                {
                    Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
                }
                this.m_hAuthTicket = HAuthTicket.Invalid;
            }
        }

        // Token: 0x0400622D RID: 25133
        private HAuthTicket m_hAuthTicket;
    }
}
