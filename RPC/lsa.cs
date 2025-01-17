﻿//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace tatertot.RPC
{
    [DebuggerDisplay("{DomainName}")]
    public class LSA_DOMAIN_INFORMATION
    {
        public string DomainName;
        public SecurityIdentifier DomainSid;
    }

    public enum SID_NAME_USE {
        SidTypeUser = 1,
        SidTypeGroup,
        SidTypeDomain,
        SidTypeAlias,
        SidTypeWellKnownGroup,
        SidTypeDeletedAccount,
        SidTypeInvalid,
        SidTypeUnknown,
        SidTypeComputer,
        SidTypeLabel
    }

    [DebuggerDisplay("{DomainName} {TranslatedName}")]
    public class LSA_LOOKUP_RESULT
    {
        public string DomainName;
        public SecurityIdentifier DomainSid;
        public string TranslatedName;
        public SID_NAME_USE Use;
    }
    
    public class lsa : rpcapi
    {

        private static byte[] MIDL_ProcFormatStringx86 = new byte[] {
            0x00,0x48,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x30,0xe0,0x00,0x00,0x00,0x00,0x38,0x00,0x40,0x00,0x44,0x02,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,
            0x18,0x01,0x00,0x00,0x06,0x00,0x70,0x00,0x04,0x00,0x08,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x01,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x02,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,
            0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x03,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x04,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x05,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x06,0x00,0x14,0x00,0x31,0x04,0x00,0x00,0x00,0x5c,0x22,0x00,0x40,0x00,0x46,0x05,0x08,0x05,0x00,0x00,0x01,0x00,
            0x00,0x00,0x0a,0x00,0x00,0x00,0x0a,0x00,0x0b,0x01,0x04,0x00,0xc0,0x00,0x48,0x00,0x08,0x00,0x08,0x00,0x10,0x01,0x0c,0x00,0xfa,0x00,0x70,0x00,0x10,0x00,
            0x08,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x07,0x00,0x10,0x00,0x30,0x40,0x00,0x00,0x00,0x00,0x2a,0x00,0x08,0x00,0x45,0x04,0x08,0x03,0x01,0x00,0x00,0x00,
            0x00,0x00,0x08,0x00,0x00,0x00,0xfe,0x00,0x48,0x00,0x04,0x00,0x0d,0x00,0x13,0x20,0x08,0x00,0x02,0x01,0x70,0x00,0x0c,0x00,0x08,0x00,0x00,0x48,0x00,0x00,
            0x00,0x00,0x08,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,
            0x09,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0a,0x00,
            0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0b,0x00,0x04,0x00,
            0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0c,0x00,0x04,0x00,0x32,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0d,0x00,0x04,0x00,0x32,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0e,0x00,0x04,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x08,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0f,0x00,0x1c,0x00,0x30,0x40,0x00,0x00,0x00,0x00,0x46,0x00,
            0x24,0x00,0x47,0x07,0x08,0x07,0x01,0x00,0x01,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0xfe,0x00,0x0b,0x01,0x04,0x00,0xf8,0x02,0x13,0x20,0x08,0x00,0x0a,0x03,
            0x1b,0x01,0x0c,0x00,0x86,0x03,0x48,0x00,0x10,0x00,0x0d,0x00,0x58,0x01,0x14,0x00,0x08,0x00,0x70,0x00,0x18,0x00,0x08,0x00,0x00
            };

        private static byte[] MIDL_ProcFormatStringx64 = new byte[] {
            0x00,0x48,0x00,0x00,0x00,0x00,0x00,0x00,0x10,0x00,0x30,0xe0,0x00,0x00,0x00,0x00,0x38,0x00,0x40,0x00,0x44,0x02,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x18,0x01,0x00,0x00,0x06,0x00,0x70,0x00,0x08,0x00,0x08,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x01,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x02,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x03,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x04,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x05,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x06,0x00,0x28,0x00,0x31,0x08,0x00,0x00,0x00,0x5c,
            0x22,0x00,0x40,0x00,0x46,0x05,0x0a,0x05,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x0a,0x00,0x00,0x00,0x0a,0x00,0x0b,0x01,0x08,0x00,0xa6,0x00,0x48,0x00,
            0x10,0x00,0x08,0x00,0x10,0x01,0x18,0x00,0xcc,0x00,0x70,0x00,0x20,0x00,0x08,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x07,0x00,0x20,0x00,0x30,0x40,0x00,0x00,
            0x00,0x00,0x2a,0x00,0x08,0x00,0x45,0x04,0x0a,0x03,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0xd0,0x00,0x48,0x00,0x08,0x00,0x0d,0x00,
            0x13,0x20,0x10,0x00,0xd4,0x00,0x70,0x00,0x18,0x00,0x08,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x08,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x09,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0a,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0b,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0c,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0d,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0e,0x00,0x08,0x00,0x32,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x40,0x00,0x0a,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x00,0x0f,0x00,0x38,0x00,0x30,0x40,0x00,0x00,0x00,0x00,0x46,0x00,
            0x24,0x00,0x47,0x07,0x0a,0x07,0x01,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0xd0,0x00,0x0b,0x01,0x08,0x00,0x74,0x02,0x13,0x20,0x10,0x00,
            0x88,0x02,0x1b,0x01,0x18,0x00,0x00,0x03,0x48,0x00,0x20,0x00,0x0d,0x00,0x58,0x01,0x28,0x00,0x08,0x00,0x70,0x00,0x30,0x00,0x08,0x00,0x00
        };

        private static byte[] MIDL_TypeFormatStringx86 = new byte[] {
            0x00,0x00,0x11,0x04,0x02,0x00,0x30,0xe1,0x00,0x00,0x12,0x08,0x05,0x5c,0x11,0x00,0xb0,0x00,0x1c,0x00,0x01,0x00,0x17,0x00,0x02,0x00,0x01,0x00,0x17,0x00,
            0x00,0x00,0x01,0x00,0x02,0x5b,0x16,0x03,0x08,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x00,0xe0,0xff,0x5b,0x06,0x06,0x08,0x5c,0x5b,0x1d,0x00,
            0x06,0x00,0x01,0x5b,0x15,0x00,0x06,0x00,0x4c,0x00,0xf4,0xff,0x5c,0x5b,0x1b,0x03,0x04,0x00,0x04,0x00,0xf9,0xff,0x01,0x00,0x08,0x5b,0x17,0x03,0x08,0x00,
            0xf0,0xff,0x02,0x02,0x4c,0x00,0xe0,0xff,0x5c,0x5b,0x1b,0x00,0x01,0x00,0x00,0x59,0x00,0x00,0x00,0x00,0x02,0x5b,0x17,0x01,0x04,0x00,0xf0,0xff,0x02,0x02,
            0x06,0x5b,0x16,0x03,0x14,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x00,0xce,0xff,0x46,0x5c,0x08,0x00,0x08,0x00,0x12,0x00,0xc4,0xff,0x46,0x5c,
            0x0c,0x00,0x0c,0x00,0x12,0x00,0xd4,0xff,0x46,0x5c,0x10,0x00,0x10,0x00,0x12,0x00,0xca,0xff,0x5b,0x02,0x02,0x06,0x08,0x08,0x08,0x08,0x5c,0x5b,0x1a,0x03,
            0x0c,0x00,0x00,0x00,0x00,0x00,0x08,0x0d,0x02,0x02,0x3e,0x5b,0x16,0x03,0x18,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x08,0x02,0x5c,0x46,0x5c,
            0x08,0x00,0x08,0x00,0x12,0x00,0x4c,0xff,0x46,0x5c,0x10,0x00,0x10,0x00,0x12,0x00,0x98,0xff,0x46,0x5c,0x14,0x00,0x14,0x00,0x12,0x00,0xc6,0xff,0x5b,0x08,
            0x08,0x08,0x08,0x08,0x08,0x5b,0x11,0x04,0x02,0x00,0x30,0xa0,0x00,0x00,0x30,0x41,0x00,0x00,0x11,0x14,0x02,0x00,0x12,0x00,0x02,0x00,0x2b,0x0d,0x26,0x00,
            0x04,0x00,0x01,0x00,0x02,0x00,0x30,0x00,0x0d,0x70,0x01,0x00,0x00,0x00,0x52,0x00,0x02,0x00,0x00,0x00,0x7a,0x00,0x03,0x00,0x00,0x00,0x9a,0x00,0x05,0x00,
            0x00,0x00,0x94,0x00,0x04,0x00,0x00,0x00,0xae,0x00,0x06,0x00,0x00,0x00,0xbe,0x00,0x07,0x00,0x00,0x00,0xd4,0x00,0x09,0x00,0x00,0x00,0xf0,0x00,0x0a,0x00,
            0x00,0x00,0xf8,0x00,0x0b,0x00,0x00,0x00,0xf8,0x00,0x0c,0x00,0x00,0x00,0x1e,0x01,0x0d,0x00,0x00,0x00,0x18,0x01,0x0e,0x00,0x00,0x00,0x5e,0x00,0xff,0xff,
            0x15,0x07,0x08,0x00,0x0b,0x5b,0x1a,0x07,0x28,0x00,0x00,0x00,0x00,0x00,0x08,0x08,0x4c,0x00,0xee,0xff,0x02,0x43,0x4c,0x00,0xe8,0xff,0x08,0x40,0x5c,0x5b,
            0xb7,0x08,0x00,0x00,0x00,0x00,0xe8,0x03,0x00,0x00,0x1b,0x03,0x04,0x00,0x19,0x00,0x08,0x00,0x00,0x00,0x08,0x5b,0x1a,0x03,0x0c,0x00,0x00,0x00,0x0a,0x00,
            0x02,0x3f,0x36,0x4c,0x00,0xdd,0xff,0x5b,0x12,0x00,0xe2,0xff,0x1c,0x01,0x02,0x00,0x17,0x55,0x02,0x00,0x01,0x00,0x17,0x55,0x00,0x00,0x01,0x00,0x05,0x5b,
            0x16,0x03,0x0c,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x00,0xe0,0xff,0x46,0x5c,0x08,0x00,0x08,0x00,0x12,0x00,0x7c,0xfe,0x5b,0x06,0x06,0x08,
            0x08,0x5b,0x16,0x03,0x08,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x00,0xc0,0xff,0x5b,0x06,0x06,0x08,0x5c,0x5b,0x1a,0x01,0x04,0x00,0x00,0x00,
            0x00,0x00,0x0d,0x5b,0x1c,0x01,0x02,0x00,0x17,0x55,0x0a,0x00,0x01,0x00,0x17,0x55,0x08,0x00,0x01,0x00,0x05,0x5b,0x16,0x03,0x10,0x00,0x4b,0x5c,0x46,0x5c,
            0x04,0x00,0x04,0x00,0x12,0x00,0x8e,0xff,0x46,0x5c,0x0c,0x00,0x0c,0x00,0x12,0x00,0xd6,0xff,0x5b,0x06,0x06,0x08,0x06,0x06,0x08,0x5b,0x15,0x07,0x10,0x00,
            0x4c,0x00,0x2c,0xff,0x4c,0x00,0x28,0xff,0x5c,0x5b,0x15,0x00,0x01,0x00,0x02,0x5b,0x15,0x00,0x02,0x00,0x02,0x02,0x5c,0x5b,0x1d,0x00,0x08,0x00,0x01,0x5b,
            0x15,0x03,0x10,0x00,0x08,0x06,0x06,0x4c,0x00,0xf1,0xff,0x5b,0x1c,0x01,0x02,0x00,0x17,0x55,0x12,0x00,0x01,0x00,0x17,0x55,0x10,0x00,0x01,0x00,0x05,0x5b,
            0x16,0x03,0x2c,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,0x12,0x00,0x2c,0xff,0x46,0x5c,0x0c,0x00,0x0c,0x00,0x12,0x00,0x74,0xff,0x46,0x5c,0x14,0x00,
            0x14,0x00,0x12,0x00,0xcc,0xff,0x46,0x5c,0x28,0x00,0x28,0x00,0x12,0x00,0xb4,0xfd,0x5b,0x06,0x06,0x08,0x06,0x06,0x08,0x06,0x06,0x08,0x4c,0x00,0xa8,0xff,
            0x08,0x5b,0x11,0x00,0x42,0x00,0xb7,0x08,0x00,0x00,0x00,0x00,0x00,0x50,0x00,0x00,0x16,0x03,0x04,0x00,0x4b,0x5c,0x46,0x5c,0x00,0x00,0x00,0x00,0x12,0x00,
            0x86,0xfd,0x5b,0x08,0x5c,0x5b,0x1b,0x03,0x04,0x00,0x19,0x00,0x00,0x00,0x01,0x00,0x4b,0x5c,0x48,0x49,0x04,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,
            0x12,0x00,0x66,0xfd,0x5b,0x4c,0x00,0xcd,0xff,0x5b,0x1a,0x03,0x08,0x00,0x00,0x00,0x08,0x00,0x4c,0x00,0xb6,0xff,0x36,0x5b,0x12,0x00,0xce,0xff,0x11,0x14,
            0x02,0x00,0x12,0x00,0x2c,0x00,0x1b,0x03,0x0c,0x00,0x19,0x00,0x00,0x00,0x01,0x00,0x4b,0x5c,0x48,0x49,0x0c,0x00,0x00,0x00,0x02,0x00,0x04,0x00,0x04,0x00,
            0x12,0x00,0x84,0xfe,0x08,0x00,0x08,0x00,0x12,0x00,0x22,0xfd,0x5b,0x4c,0x00,0x89,0xfe,0x5b,0x16,0x03,0x0c,0x00,0x4b,0x5c,0x46,0x5c,0x04,0x00,0x04,0x00,
            0x12,0x00,0xc8,0xff,0x5b,0x08,0x08,0x08,0x5c,0x5b,0x11,0x00,0x32,0x00,0xb7,0x08,0x00,0x00,0x00,0x00,0x00,0x50,0x00,0x00,0x1a,0x03,0x10,0x00,0x00,0x00,
            0x00,0x00,0x0d,0x4c,0x00,0x77,0xfe,0x08,0x5c,0x5b,0x21,0x03,0x00,0x00,0x19,0x00,0x00,0x00,0x01,0x00,0xff,0xff,0xff,0xff,0x00,0x00,0x4c,0x00,0xde,0xff,
            0x5c,0x5b,0x1a,0x03,0x08,0x00,0x00,0x00,0x08,0x00,0x4c,0x00,0xc6,0xff,0x36,0x5b,0x12,0x00,0xda,0xff,0x11,0x08,0x08,0x5c,0x00
        };

        private static byte[] MIDL_TypeFormatStringx64 = new byte[] {
            0x00,0x00,0x11,0x04,0x02,0x00,0x30,0xe1,0x00,0x00,0x12,0x08,0x05,0x5c,0x11,0x00,0x96,0x00,0x1c,0x00,0x01,0x00,0x17,0x00,0x02,0x00,0x01,0x00,0x17,0x00,
            0x00,0x00,0x01,0x00,0x02,0x5b,0x1a,0x03,0x10,0x00,0x00,0x00,0x08,0x00,0x06,0x06,0x40,0x36,0x5c,0x5b,0x12,0x00,0xde,0xff,0x1d,0x00,0x06,0x00,0x01,0x5b,
            0x15,0x00,0x06,0x00,0x4c,0x00,0xf4,0xff,0x5c,0x5b,0x1b,0x03,0x04,0x00,0x04,0x00,0xf9,0xff,0x01,0x00,0x08,0x5b,0x17,0x03,0x08,0x00,0xf0,0xff,0x02,0x02,
            0x4c,0x00,0xe0,0xff,0x5c,0x5b,0x1b,0x00,0x01,0x00,0x00,0x59,0x00,0x00,0x00,0x00,0x02,0x5b,0x17,0x01,0x04,0x00,0xf0,0xff,0x02,0x02,0x06,0x5b,0x1a,0x03,
            0x28,0x00,0x00,0x00,0x0c,0x00,0x02,0x02,0x06,0x40,0x36,0x36,0x36,0x36,0x5c,0x5b,0x12,0x00,0xc8,0xff,0x12,0x00,0xc4,0xff,0x12,0x00,0xda,0xff,0x12,0x00,
            0xd6,0xff,0x1a,0x03,0x0c,0x00,0x00,0x00,0x00,0x00,0x08,0x0d,0x02,0x02,0x3e,0x5b,0x1a,0x03,0x30,0x00,0x00,0x00,0x0c,0x00,0x08,0x40,0x36,0x36,0x08,0x40,
            0x36,0x36,0x5c,0x5b,0x12,0x08,0x02,0x5c,0x12,0x00,0x66,0xff,0x12,0x00,0xb4,0xff,0x12,0x00,0xd2,0xff,0x11,0x04,0x02,0x00,0x30,0xa0,0x00,0x00,0x30,0x41,
            0x00,0x00,0x11,0x14,0x02,0x00,0x12,0x00,0x02,0x00,0x2b,0x0d,0x26,0x00,0x08,0x00,0x01,0x00,0x02,0x00,0x48,0x00,0x0d,0x70,0x01,0x00,0x00,0x00,0x52,0x00,
            0x02,0x00,0x00,0x00,0x7a,0x00,0x03,0x00,0x00,0x00,0xae,0x00,0x05,0x00,0x00,0x00,0xba,0x00,0x04,0x00,0x00,0x00,0xc6,0x00,0x06,0x00,0x00,0x00,0xce,0x00,
            0x07,0x00,0x00,0x00,0xd2,0x00,0x09,0x00,0x00,0x00,0xde,0x00,0x0a,0x00,0x00,0x00,0xe6,0x00,0x0b,0x00,0x00,0x00,0xe6,0x00,0x0c,0x00,0x00,0x00,0xfa,0x00,
            0x0d,0x00,0x00,0x00,0xf4,0x00,0x0e,0x00,0x00,0x00,0x84,0x00,0xff,0xff,0x15,0x07,0x08,0x00,0x0b,0x5b,0x1a,0x07,0x28,0x00,0x00,0x00,0x00,0x00,0x08,0x08,
            0x4c,0x00,0xee,0xff,0x02,0x43,0x4c,0x00,0xe8,0xff,0x08,0x40,0x5c,0x5b,0xb7,0x08,0x00,0x00,0x00,0x00,0xe8,0x03,0x00,0x00,0x1b,0x03,0x04,0x00,0x19,0x00,
            0x10,0x00,0x00,0x00,0x08,0x5b,0x1a,0x03,0x18,0x00,0x00,0x00,0x0c,0x00,0x02,0x43,0x36,0x4c,0x00,0xdd,0xff,0x40,0x5c,0x5b,0x12,0x00,0xe0,0xff,0x1c,0x01,
            0x02,0x00,0x17,0x55,0x02,0x00,0x01,0x00,0x17,0x55,0x00,0x00,0x01,0x00,0x05,0x5b,0x1a,0x03,0x10,0x00,0x00,0x00,0x08,0x00,0x06,0x06,0x40,0x36,0x5c,0x5b,
            0x12,0x00,0xde,0xff,0x1a,0x03,0x18,0x00,0x00,0x00,0x08,0x00,0x4c,0x00,0xe4,0xff,0x36,0x5b,0x12,0x00,0x9a,0xfe,0x1a,0x03,0x18,0x00,0x00,0x00,0x08,0x00,
            0x4c,0x00,0xd2,0xff,0x36,0x5b,0x12,0x00,0x88,0xfe,0x1a,0x03,0x10,0x00,0x00,0x00,0x00,0x00,0x4c,0x00,0xc0,0xff,0x5c,0x5b,0x1a,0x01,0x04,0x00,0x00,0x00,
            0x00,0x00,0x0d,0x5b,0x1a,0x03,0x20,0x00,0x00,0x00,0x00,0x00,0x4c,0x00,0xa8,0xff,0x4c,0x00,0xa4,0xff,0x5c,0x5b,0x15,0x07,0x10,0x00,0x4c,0x00,0x3e,0xff,
            0x4c,0x00,0x3a,0xff,0x5c,0x5b,0x15,0x00,0x01,0x00,0x02,0x5b,0x15,0x00,0x02,0x00,0x02,0x02,0x5c,0x5b,0x1d,0x00,0x08,0x00,0x01,0x5b,0x15,0x03,0x10,0x00,
            0x08,0x06,0x06,0x4c,0x00,0xf1,0xff,0x5b,0x1a,0x03,0x48,0x00,0x00,0x00,0x14,0x00,0x4c,0x00,0x68,0xff,0x4c,0x00,0x64,0xff,0x4c,0x00,0x60,0xff,0x4c,0x00,
            0xde,0xff,0x36,0x5b,0x12,0x00,0x12,0xfe,0x11,0x00,0x30,0x00,0xb7,0x08,0x00,0x00,0x00,0x00,0x00,0x50,0x00,0x00,0x1a,0x03,0x08,0x00,0x00,0x00,0x04,0x00,
            0x36,0x5b,0x12,0x00,0xf6,0xfd,0x21,0x03,0x00,0x00,0x19,0x00,0x00,0x00,0x01,0x00,0xff,0xff,0xff,0xff,0x00,0x00,0x4c,0x00,0xe0,0xff,0x5c,0x5b,0x1a,0x03,
            0x10,0x00,0x00,0x00,0x0a,0x00,0x4c,0x00,0xc8,0xff,0x40,0x36,0x5c,0x5b,0x12,0x00,0xd8,0xff,0x11,0x14,0x02,0x00,0x12,0x00,0x2a,0x00,0x1a,0x03,0x18,0x00,
            0x00,0x00,0x08,0x00,0x4c,0x00,0xfc,0xfe,0x36,0x5b,0x12,0x00,0xb2,0xfd,0x21,0x03,0x00,0x00,0x19,0x00,0x00,0x00,0x01,0x00,0xff,0xff,0xff,0xff,0x00,0x00,
            0x4c,0x00,0xdc,0xff,0x5c,0x5b,0x1a,0x03,0x18,0x00,0x00,0x00,0x08,0x00,0x08,0x40,0x36,0x08,0x40,0x5b,0x12,0x00,0xda,0xff,0x11,0x00,0x34,0x00,0xb7,0x08,
            0x00,0x00,0x00,0x00,0x00,0x50,0x00,0x00,0x1a,0x03,0x20,0x00,0x00,0x00,0x00,0x00,0x0d,0x40,0x4c,0x00,0xb2,0xfe,0x08,0x40,0x5c,0x5b,0x21,0x03,0x00,0x00,
            0x19,0x00,0x00,0x00,0x01,0x00,0xff,0xff,0xff,0xff,0x00,0x00,0x4c,0x00,0xdc,0xff,0x5c,0x5b,0x1a,0x03,0x10,0x00,0x00,0x00,0x0a,0x00,0x4c,0x00,0xc4,0xff,
            0x40,0x36,0x5c,0x5b,0x12,0x00,0xd8,0xff,0x11,0x08,0x08,0x5c,0x00
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSAPR_OBJECT_ATTRIBUTES
        {
            public UInt32 Length;
            public IntPtr RootDirectory;
            public IntPtr ObjectName;
            public UInt32 Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LSAPR_POLICY_ACCOUNT_DOM_INFO
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr buffer;
            public IntPtr DomainSid;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSAPR_SID_ENUM_BUFFER
        {
            public UInt32 Entries;
            public IntPtr SidInfo;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct LSAPR_REFERENCED_DOMAIN_LIST
        {
            public UInt32 Entries;
            public IntPtr Domains;
            public UInt32 MaxEntries;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LSAPR_TRUST_INFORMATION
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr buffer;
            public IntPtr Sid;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct LSAPR_TRANSLATED_NAMES
        {
            public UInt32 Entries;
            public IntPtr Names;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LSAPR_TRANSLATED_NAME
        {
            public IntPtr Use;
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr buffer;
            public UInt32 DomainIndex;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public lsa()
        {
            Guid interfaceId = new Guid("12345778-1234-ABCD-EF00-0123456789AB");
            if (IntPtr.Size == 8)
            {
                InitializeStub(interfaceId, MIDL_ProcFormatStringx64, MIDL_TypeFormatStringx64, "\\pipe\\lsarpc", 0);
            }
            else
            {
                InitializeStub(interfaceId, MIDL_ProcFormatStringx86, MIDL_TypeFormatStringx86, "\\pipe\\lsarpc", 0);
            }
			UseNullSession = true;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        ~lsa()
        {
            freeStub();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Int32 LsarOpenPolicy(string SystemName, UInt32 DesiredAccess, out IntPtr PolicyHandle)
        {
            IntPtr intptrSystemName = Marshal.StringToHGlobalUni(SystemName);
            LSAPR_OBJECT_ATTRIBUTES objectAttributes = new LSAPR_OBJECT_ATTRIBUTES();
            PolicyHandle = IntPtr.Zero;
            IntPtr result = IntPtr.Zero;
            try
            {
                PolicyHandle = IntPtr.Zero;
                if (IntPtr.Size == 8)
                {
                    result = NativeMethods.NdrClientCall2x64(GetStubHandle(), GetProcStringHandle(194), intptrSystemName, ref objectAttributes, DesiredAccess, out PolicyHandle);
                }
                else
                {
                    IntPtr tempValue1 = new IntPtr();
                    GCHandle handle1 = GCHandle.Alloc(tempValue1, GCHandleType.Pinned);
                    IntPtr tempValuePointer1 = handle1.AddrOfPinnedObject();
                    GCHandle handle2 = GCHandle.Alloc(objectAttributes, GCHandleType.Pinned);
                    IntPtr tempValuePointer2 = handle2.AddrOfPinnedObject();
                    try
                    {
                        result = CallNdrClientCall2x86(182, intptrSystemName, tempValuePointer2, new IntPtr((int)DesiredAccess), tempValuePointer1);
                        // each pinvoke work on a copy of the arguments (without an out specifier)
                        // get back the data
                        PolicyHandle = Marshal.ReadIntPtr(tempValuePointer1);
                    }
                    finally
                    {
                        handle1.Free();
                        handle2.Free();
                    }
                }
            }
            catch (SEHException)
            {
                Trace.WriteLine("LsarOpenPolicy failed 0x" + Marshal.GetExceptionCode().ToString("x"));
                return Marshal.GetExceptionCode();
            }
            finally
            {
                if (intptrSystemName != IntPtr.Zero)
                    Marshal.FreeHGlobal(intptrSystemName);
            }
            return (int) result.ToInt64();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Int32 LsarClose(ref IntPtr ServerHandle)
        {
            IntPtr result = IntPtr.Zero;
            try
            {
                if (IntPtr.Size == 8)
                {
                    result = NativeMethods.NdrClientCall2x64(GetStubHandle(), GetProcStringHandle(0), ref ServerHandle);
                }
                else
                {
                    IntPtr tempValue = ServerHandle;
                    GCHandle handle = GCHandle.Alloc(tempValue, GCHandleType.Pinned);
                    IntPtr tempValuePointer = handle.AddrOfPinnedObject();
                    try
                    {
                        result = CallNdrClientCall2x86(0, tempValuePointer);
                        // each pinvoke work on a copy of the arguments (without an out specifier)
                        // get back the data
                        ServerHandle = Marshal.ReadIntPtr(tempValuePointer);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }
            catch (SEHException)
            {
                Trace.WriteLine("LsarClose failed 0x" + Marshal.GetExceptionCode().ToString("x"));
                return Marshal.GetExceptionCode();
            }
            return (int) result.ToInt64();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Int32 LsarQueryInformationPolicy(IntPtr PolicyHandle, UInt32 InformationClass, out LSA_DOMAIN_INFORMATION PolicyInformation)
        {
            IntPtr result = IntPtr.Zero;
            try
            {
                IntPtr IntPtrPolicyInformation = IntPtr.Zero;
                if (IntPtr.Size == 8)
                {
                    result = NativeMethods.NdrClientCall2x64(GetStubHandle(), GetProcStringHandle(256), PolicyHandle, InformationClass, out IntPtrPolicyInformation);
                }
                else
                {
                    IntPtr tempValue1 = IntPtr.Zero;
                    GCHandle handle1 = GCHandle.Alloc(tempValue1, GCHandleType.Pinned);
                    IntPtr tempValuePointer1 = handle1.AddrOfPinnedObject();
                    try
                    {
                        result = CallNdrClientCall2x86(242, PolicyHandle, new IntPtr(InformationClass), tempValuePointer1);
                        // each pinvoke work on a copy of the arguments (without an out specifier)
                        // get back the data
                        IntPtrPolicyInformation = Marshal.ReadIntPtr(tempValuePointer1);
                    }
                    finally
                    {
                        handle1.Free();
                    }
                }
                PolicyInformation = Unmarshal_LSAPR_POLICY_ACCOUNT_DOM_INFO(IntPtrPolicyInformation);
            }
            catch (SEHException)
            {
                PolicyInformation = null;
                Trace.WriteLine("LsarQueryInformationPolicy failed 0x" + Marshal.GetExceptionCode().ToString("x"));
                return Marshal.GetExceptionCode();
            }
            return (int) result.ToInt64();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private LSA_DOMAIN_INFORMATION Unmarshal_LSAPR_POLICY_ACCOUNT_DOM_INFO(IntPtr IntPtrPolicyInformation)
        {
            if (IntPtrPolicyInformation == IntPtr.Zero)
                return null;
            LSAPR_POLICY_ACCOUNT_DOM_INFO Buffer = (LSAPR_POLICY_ACCOUNT_DOM_INFO)Marshal.PtrToStructure(IntPtrPolicyInformation, typeof(LSAPR_POLICY_ACCOUNT_DOM_INFO));
            LSA_DOMAIN_INFORMATION output = new LSA_DOMAIN_INFORMATION();
            output.DomainName = Marshal.PtrToStringUni(Buffer.buffer, Buffer.Length / 2);
            output.DomainSid = new SecurityIdentifier(Buffer.DomainSid);
            
            if (Buffer.buffer != IntPtr.Zero && Buffer.MaximumLength > 0)
                FreeMemory(Buffer.buffer);
            if (Buffer.DomainSid != IntPtr.Zero)
                FreeMemory(Buffer.DomainSid);
            FreeMemory(IntPtrPolicyInformation);
            return output;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public Int32 LsarLookupSids(IntPtr PolicyHandle, SecurityIdentifier[] SidEnumBuffer, out LSA_LOOKUP_RESULT[] LookupResult, UInt32 LookupLevel,out UInt32 MappedCount)
        {
            List<GCHandle> HandleToFree = new List<GCHandle>();
            IntPtr result = IntPtr.Zero;
            LookupResult = null;
            MappedCount = 0;
            try
            {
                IntPtr IntPtrReferencedDomains = IntPtr.Zero;
                LSAPR_TRANSLATED_NAMES TranslatedNames = new LSAPR_TRANSLATED_NAMES();
                GCHandle handleTranslatedNames = GCHandle.Alloc(TranslatedNames, GCHandleType.Pinned);
                // translatedNamesValuePointer points to a copy of TranslatedNames
                IntPtr IntPtrTranslatedNames = handleTranslatedNames.AddrOfPinnedObject();
                HandleToFree.Add(handleTranslatedNames);

                LSAPR_SID_ENUM_BUFFER enumBuffer = Marshal_LSAPR_SID_ENUM_BUFFER(SidEnumBuffer, HandleToFree);
                if (IntPtr.Size == 8)
                {
                    result = NativeMethods.NdrClientCall2x64(GetStubHandle(), GetProcStringHandle(522), PolicyHandle, enumBuffer, out IntPtrReferencedDomains, IntPtrTranslatedNames, LookupLevel, out MappedCount);
                }
                else
                {
                    GCHandle handle1 = GCHandle.Alloc(enumBuffer, GCHandleType.Pinned);
                    IntPtr tempValuePointer1 = handle1.AddrOfPinnedObject();
                    IntPtr tempValue2 = IntPtr.Zero;
                    GCHandle handle2 = GCHandle.Alloc(tempValue2, GCHandleType.Pinned);
                    IntPtr tempValuePointer2 = handle2.AddrOfPinnedObject();
                    
                    IntPtr tempValue4 = IntPtr.Zero;
                    GCHandle handle4 = GCHandle.Alloc(tempValue4, GCHandleType.Pinned);
                    IntPtr tempValuePointer4 = handle4.AddrOfPinnedObject();
                    try
                    {
                        result = CallNdrClientCall2x86(492, PolicyHandle, tempValuePointer1, tempValuePointer2, IntPtrTranslatedNames, new IntPtr(LookupLevel), tempValuePointer4);
                        // each pinvoke work on a copy of the arguments (without an out specifier)
                        // get back the data
                        IntPtrReferencedDomains = Marshal.ReadIntPtr(tempValuePointer2);
                        MappedCount = (UInt32)Marshal.ReadInt32(tempValuePointer4);
                    }
                    finally
                    {
                        handle1.Free();
                        handle2.Free();
                        handle4.Free();
                    }
                }
                if (result == IntPtr.Zero || result == new IntPtr(0x00000107))
                {
                    LookupResult = Marshal_LsarLookupSids_Output(IntPtrReferencedDomains, IntPtrTranslatedNames);
                }
            }
            catch (SEHException)
            {
                Trace.WriteLine("LsarLookupSids failed 0x" + Marshal.GetExceptionCode().ToString("x"));
                return Marshal.GetExceptionCode();
            }
            finally
            {
                foreach (GCHandle handle in HandleToFree)
                {
                    handle.Free();
                }
            }
            return (int) result.ToInt64();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private LSAPR_SID_ENUM_BUFFER Marshal_LSAPR_SID_ENUM_BUFFER(SecurityIdentifier[] SidEnumBuffer, List<GCHandle> HandleToFree)
        {
            LSAPR_SID_ENUM_BUFFER output = new LSAPR_SID_ENUM_BUFFER();
            output.Entries = (UInt32) SidEnumBuffer.Length;
            IntPtr[] sidPtr = new IntPtr[SidEnumBuffer.Length];
            for (int i = 0; i < SidEnumBuffer.Length; i++)
            {
                byte[] sid = new byte[SidEnumBuffer[i].BinaryLength];
                SidEnumBuffer[i].GetBinaryForm(sid, 0);
                GCHandle handlesid = GCHandle.Alloc(sid, GCHandleType.Pinned);
                HandleToFree.Add(handlesid);
                sidPtr[i] = handlesid.AddrOfPinnedObject();
            }
            GCHandle handle = GCHandle.Alloc(sidPtr, GCHandleType.Pinned);
            HandleToFree.Add(handle);
            output.SidInfo = handle.AddrOfPinnedObject();
            return output;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private LSA_LOOKUP_RESULT[] Marshal_LsarLookupSids_Output(IntPtr IntPtrReferencedDomains, IntPtr IntPtrTranslatedNames)
        {
            if (IntPtrReferencedDomains == IntPtr.Zero || IntPtrTranslatedNames == IntPtr.Zero)
                return null;
            LSAPR_REFERENCED_DOMAIN_LIST ReferencedDomains = (LSAPR_REFERENCED_DOMAIN_LIST)Marshal.PtrToStructure(IntPtrReferencedDomains, typeof(LSAPR_REFERENCED_DOMAIN_LIST));
            LSAPR_TRANSLATED_NAMES TranslatedNames = (LSAPR_TRANSLATED_NAMES)Marshal.PtrToStructure(IntPtrTranslatedNames, typeof(LSAPR_TRANSLATED_NAMES));


            int SizeTranslatedName = Marshal.SizeOf(typeof(LSAPR_TRANSLATED_NAME));
            int SizeTrustInformation = Marshal.SizeOf(typeof(LSAPR_TRUST_INFORMATION));

            string[] referencedDomainsString = new string[ReferencedDomains.Entries];
            SecurityIdentifier[] referencedDomainsSid = new SecurityIdentifier[ReferencedDomains.Entries];
            for (UInt32 i = 0; i < ReferencedDomains.Entries; i++)
            {
                LSAPR_TRUST_INFORMATION trustInformation = (LSAPR_TRUST_INFORMATION)Marshal.PtrToStructure(new IntPtr(ReferencedDomains.Domains.ToInt64() + SizeTrustInformation * i), typeof(LSAPR_TRUST_INFORMATION));

                if (trustInformation.buffer != IntPtr.Zero)
                    referencedDomainsString[i] = Marshal.PtrToStringUni(trustInformation.buffer, trustInformation.Length / 2);
                if (trustInformation.Sid != null)
                    referencedDomainsSid[i] = new SecurityIdentifier(trustInformation.Sid);

                if (trustInformation.buffer != IntPtr.Zero && trustInformation.MaximumLength > 0)
                    FreeMemory(trustInformation.buffer);
                if (trustInformation.Sid != IntPtr.Zero)
                    FreeMemory(trustInformation.Sid);
            }

            LSA_LOOKUP_RESULT[] output = new LSA_LOOKUP_RESULT[TranslatedNames.Entries];
            for (UInt32 i = 0; i < TranslatedNames.Entries; i++)
            {
                LSAPR_TRANSLATED_NAME translatedName = (LSAPR_TRANSLATED_NAME)Marshal.PtrToStructure(new IntPtr(TranslatedNames.Names.ToInt64() + SizeTranslatedName * i), typeof(LSAPR_TRANSLATED_NAME));
                output[i] = new LSA_LOOKUP_RESULT();

                if (translatedName.buffer != IntPtr.Zero)
                    output[i].TranslatedName = Marshal.PtrToStringUni(translatedName.buffer, translatedName.Length / 2);
                output[i].Use = (SID_NAME_USE) translatedName.Use;
                output[i].DomainName = referencedDomainsString[translatedName.DomainIndex];
                output[i].DomainSid = referencedDomainsSid[translatedName.DomainIndex];
                
                if (translatedName.buffer != IntPtr.Zero && translatedName.MaximumLength > 0)
                    FreeMemory(translatedName.buffer);
            }

            FreeMemory(ReferencedDomains.Domains);
            FreeMemory(TranslatedNames.Names);
            FreeMemory(IntPtrReferencedDomains);
            return output;
        }
    }
}
