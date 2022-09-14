using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZnFramework;

public class TestVar : MonoBehaviour
{
    private VarInt a;
    private VarBool b;
    private VarByte c;
    private VarBytes d;
    private VarColor e;
    private VarFloat f;
    private VarLong g;
    private VarString h;

    // Start is called before the first frame update
    void Start()
    {
        a = VarInt.Alloc(10);
        VarInt.Alloc(100);
        // b = VarBool.Alloc(true);
        // c = VarByte.Alloc(1);
        // d = VarBytes.Alloc(new byte[2] {1, 2});
        // e = VarColor.Alloc(new Color(1, 1, 1, 1));
        // f = VarFloat.Alloc(1.2f);
        // g = VarLong.Alloc(10000000);
        // h = VarString.Alloc("Arycs");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int aa = a;
            // bool bb = b;
            // byte cc = c;
            // byte[] dd = d;
            // Color ee = e;
            // float ff = f;
            // long gg = g;
            // string hh = h;

            // Debug.Log($"{aa},----{bb},----{cc},----{dd},----{ee},----{ff},----{gg},----{hh}");

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            a.Release();
            // b.Release();
            // c.Release();
            // d.Release();
            // e.Release();
            // f.Release();
            // g.Release();
            // h.Release();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            var aa = VarInt.Alloc(100);
        }
    }
}