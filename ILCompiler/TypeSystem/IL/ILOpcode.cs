﻿namespace ILCompiler.TypeSystem.IL
{
    public enum ILOpcode
    {
        nop = 0x00,
        break_ = 0x01,
        ldarg_0 = 0x02,
        ldarg_1 = 0x03,
        ldarg_2 = 0x04,
        ldarg_3 = 0x05,
        ldloc_0 = 0x06,
        ldloc_1 = 0x07,
        ldloc_2 = 0x08,
        ldloc_3 = 0x09,
        stloc_0 = 0x0a,
        stloc_1 = 0x0b,
        stloc_2 = 0x0c,
        stloc_3 = 0x0d,
        ldarg_s = 0x0e,
        ldarga_s = 0x0f,
        starg_s = 0x10,
        ldloc_s = 0x11,
        ldloca_s = 0x12,
        stloc_s = 0x13,
        ldnull = 0x14,
        ldc_i4_m1 = 0x15,
        ldc_i4_0 = 0x16,
        ldc_i4_1 = 0x17,
        ldc_i4_2 = 0x18,
        ldc_i4_3 = 0x19,
        ldc_i4_4 = 0x1a,
        ldc_i4_5 = 0x1b,
        ldc_i4_6 = 0x1c,
        ldc_i4_7 = 0x1d,
        ldc_i4_8 = 0x1e,
        ldc_i4_s = 0x1f,
        ldc_i4 = 0x20,
        ldc_i8 = 0x21,
        ldc_r4 = 0x22,
        ldc_r8 = 0x23,
        dup = 0x25,
        pop = 0x26,
        jmp = 0x27,
        call = 0x28,
        calli = 0x29,
        ret = 0x2a,
        br_s = 0x2b,
        brfalse_s = 0x2c,
        brtrue_s = 0x2d,
        beq_s = 0x2e,
        bge_s = 0x2f,
        bgt_s = 0x30,
        ble_s = 0x31,
        blt_s = 0x32,
        bne_un_s = 0x33,
        bge_un_s = 0x34,
        bgt_un_s = 0x35,
        ble_un_s = 0x36,
        blt_un_s = 0x37,
        br = 0x38,
        brfalse = 0x39,
        brtrue = 0x3a,
        beq = 0x3b,
        bge = 0x3c,
        bgt = 0x3d,
        ble = 0x3e,
        blt = 0x3f,
        bne_un = 0x40,
        bge_un = 0x41,
        bgt_un = 0x42,
        ble_un = 0x43,
        blt_un = 0x44,
        switch_ = 0x45,
        ldind_i1 = 0x46,
        ldind_u1 = 0x47,
        ldind_i2 = 0x48,
        ldind_u2 = 0x49,
        ldind_i4 = 0x4a,
        ldind_u4 = 0x4b,
        ldind_i8 = 0x4c,
        ldind_i = 0x4d,
        ldind_r4 = 0x4e,
        ldind_r8 = 0x4f,
        ldind_ref = 0x50,
        stind_ref = 0x51,
        stind_i1 = 0x52,
        stind_i2 = 0x53,
        stind_i4 = 0x54,
        stind_i8 = 0x55,
        stind_r4 = 0x56,
        stind_r8 = 0x57,
        add = 0x58,
        sub = 0x59,
        mul = 0x5a,
        div = 0x5b,
        div_un = 0x5c,
        rem = 0x5d,
        rem_un = 0x5e,
        and = 0x5f,
        or = 0x60,
        xor = 0x61,
        shl = 0x62,
        shr = 0x63,
        shr_un = 0x64,
        neg = 0x65,
        not = 0x66,
        conv_i1 = 0x67,
        conv_i2 = 0x68,
        conv_i4 = 0x69,
        conv_i8 = 0x6a,
        conv_r4 = 0x6b,
        conv_r8 = 0x6c,
        conv_u4 = 0x6d,
        conv_u8 = 0x6e,
        callvirt = 0x6f,
        cpobj = 0x70,
        ldobj = 0x71,
        ldstr = 0x72,
        newobj = 0x73,
        castclass = 0x74,
        isinst = 0x75,
        conv_r_un = 0x76,
        unbox = 0x79,
        throw_ = 0x7a,
        ldfld = 0x7b,
        ldflda = 0x7c,
        stfld = 0x7d,
        ldsfld = 0x7e,
        ldsflda = 0x7f,
        stsfld = 0x80,
        stobj = 0x81,
        conv_ovf_i1_un = 0x82,
        conv_ovf_i2_un = 0x83,
        conv_ovf_i4_un = 0x84,
        conv_ovf_i8_un = 0x85,
        conv_ovf_u1_un = 0x86,
        conv_ovf_u2_un = 0x87,
        conv_ovf_u4_un = 0x88,
        conv_ovf_u8_un = 0x89,
        conv_ovf_i_un = 0x8a,
        conv_ovf_u_un = 0x8b,
        box = 0x8c,
        newarr = 0x8d,
        ldlen = 0x8e,
        ldelema = 0x8f,
        ldelem_i1 = 0x90,
        ldelem_u1 = 0x91,
        ldelem_i2 = 0x92,
        ldelem_u2 = 0x93,
        ldelem_i4 = 0x94,
        ldelem_u4 = 0x95,
        ldelem_i8 = 0x96,
        ldelem_i = 0x97,
        ldelem_r4 = 0x98,
        ldelem_r8 = 0x99,
        ldelem_ref = 0x9a,
        stelem_i = 0x9b,
        stelem_i1 = 0x9c,
        stelem_i2 = 0x9d,
        stelem_i4 = 0x9e,
        stelem_i8 = 0x9f,
        stelem_r4 = 0xa0,
        stelem_r8 = 0xa1,
        stelem_ref = 0xa2,
        ldelem = 0xa3,
        stelem = 0xa4,
        unbox_any = 0xa5,
        conv_ovf_i1 = 0xb3,
        conv_ovf_u1 = 0xb4,
        conv_ovf_i2 = 0xb5,
        conv_ovf_u2 = 0xb6,
        conv_ovf_i4 = 0xb7,
        conv_ovf_u4 = 0xb8,
        conv_ovf_i8 = 0xb9,
        conv_ovf_u8 = 0xba,
        refanyval = 0xc2,
        ckfinite = 0xc3,
        mkrefany = 0xc6,
        ldtoken = 0xd0,
        conv_u2 = 0xd1,
        conv_u1 = 0xd2,
        conv_i = 0xd3,
        conv_ovf_i = 0xd4,
        conv_ovf_u = 0xd5,
        add_ovf = 0xd6,
        add_ovf_un = 0xd7,
        mul_ovf = 0xd8,
        mul_ovf_un = 0xd9,
        sub_ovf = 0xda,
        sub_ovf_un = 0xdb,
        endfinally = 0xdc,
        leave = 0xdd,
        leave_s = 0xde,
        stind_i = 0xdf,
        conv_u = 0xe0,
        prefix1 = 0xfe,
        arglist = 0x100,
        ceq = 0x101,
        cgt = 0x102,
        cgt_un = 0x103,
        clt = 0x104,
        clt_un = 0x105,
        ldftn = 0x106,
        ldvirtftn = 0x107,
        ldarg = 0x109,
        ldarga = 0x10a,
        starg = 0x10b,
        ldloc = 0x10c,
        ldloca = 0x10d,
        stloc = 0x10e,
        localloc = 0x10f,
        endfilter = 0x111,
        unaligned = 0x112,
        volatile_ = 0x113,
        tail = 0x114,
        initobj = 0x115,
        constrained = 0x116,
        cpblk = 0x117,
        initblk = 0x118,
        no = 0x119,
        rethrow = 0x11a,
        sizeof_ = 0x11c,
        refanytype = 0x11d,
        readonly_ = 0x11e,
    }
}