﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libplctag;
using libplctag.DataTypes;

namespace libplctag_2
{
    class Program
    {
        const string PATH = "1,0";
        private static PlcType auxTipoPlc;

        static PlcType TipoPlc(string ssTipoPlc)
        {
            switch (ssTipoPlc)
            {
                case "ControlLogix":
                    return PlcType.ControlLogix;
                case "LogixPccc":
                    return PlcType.LogixPccc;
                case "Micro800":
                    return PlcType.Micro800;
                case "MicroLogix":
                    return PlcType.MicroLogix;
                case "Plc5":
                    return PlcType.Plc5;
                case "Slc500":
                    return PlcType.Slc500;
                default:
                    return PlcType.ControlLogix;
            }
        }
        static void MssWriteCLP(out bool ssStatus, out string ssReturn, string ssHost, string ssNomeTag, string ssTipo, string ssTipoPlc, int ssTimeout, string ssValor)
        {
            ssStatus = false;
            ssReturn = "";

            try
            {
                // Tipo do PLC
                auxTipoPlc = TipoPlc(ssTipoPlc);

                // Tipo da Tag
                switch (ssTipo)
                {
                    case "Bool":
                        var boolTag = new Tag<BoolPlcMapper, bool>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };

                        if (bool.Parse(ssValor) == true)
                        {
                            boolTag.Value = bool.Parse(ssValor);
                            boolTag.Write();
                        }
                        else
                        {
                            // Corrigir bug que nao faz mudar de true para false
                            var sintTag2 = new Tag<SintPlcMapper, sbyte>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                            sintTag2.Value = 0;
                            sintTag2.Write();
                        }

                        boolTag.Read();
                        ssReturn = boolTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (bool.Parse(ssValor) == boolTag.Value) ? true : false;

                        break;

                    case "Sint":
                        var sintTag = new Tag<SintPlcMapper, sbyte>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        sintTag.Value = sbyte.Parse(ssValor);
                        sintTag.Write();
                        sintTag.Read();
                        ssReturn = sintTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (sbyte.Parse(ssValor) == sintTag.Value) ? true : false;

                        break;

                    case "Int":
                        var intTag = new Tag<IntPlcMapper, short>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        intTag.Value = short.Parse(ssValor);
                        intTag.Write();
                        intTag.Read();
                        ssReturn = intTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (short.Parse(ssValor) == intTag.Value) ? true : false;

                        break;

                    case "Dint":
                        var dintTag = new Tag<DintPlcMapper, int>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        dintTag.Value = int.Parse(ssValor);
                        dintTag.Write();
                        dintTag.Read();
                        ssReturn = dintTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (int.Parse(ssValor) == int.Parse(ssValor)) ? true : false;

                        break;

                    case "Lint":
                        var lintTag = new Tag<LintPlcMapper, long>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        lintTag.Value = long.Parse(ssValor);
                        lintTag.Write();
                        lintTag.Read();
                        ssReturn = lintTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (long.Parse(ssValor) == lintTag.Value) ? true : false;

                        break;

                    case "Real":
                        var realTag = new Tag<RealPlcMapper, float>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        realTag.Value = float.Parse(ssValor);
                        realTag.Write();
                        realTag.Read();
                        ssReturn = realTag.Value.ToString("F").Replace(",", ".");

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (float.Parse(ssValor) == realTag.Value) ? true : false;

                        break;

                    case "String":
                        var stringTag = new Tag<StringPlcMapper, string>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        stringTag.Value = ssValor;
                        stringTag.Write();
                        stringTag.Read();
                        ssReturn = stringTag.Value.ToString();

                        // Verifica se valor gravado é o mesmo que foi lido
                        ssStatus = (ssValor == stringTag.Value) ? true : false;

                        break;
                }
            }
            catch (Exception Ex)
            {
                ssStatus = false;
                ssReturn = Ex.ToString();
            }
        } // MssWriteCLP

        static void MssReadCLP(out bool ssStatus, out string ssReturn, string ssHost, string ssNomeTag, string ssTipo, string ssTipoPlc, int ssTimeout)
        {
            ssReturn = "";
            try
            {
                ssStatus = true;

                // Tipo do PLC
                auxTipoPlc = TipoPlc(ssTipoPlc);

                // Tipo da Tag
                switch (ssTipo)
                {
                    case "Bool":
                        var boolTag = new Tag<BoolPlcMapper, bool>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        boolTag.Read();
                        ssReturn = boolTag.Value.ToString();
                        break;

                    case "Sint":
                        var sintTag = new Tag<SintPlcMapper, sbyte>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        sintTag.Read();
                        ssReturn = sintTag.Value.ToString();
                        break;

                    case "Int":
                        var intTag = new Tag<IntPlcMapper, short>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        intTag.Read();
                        ssReturn = intTag.Value.ToString();
                        break;

                    case "Dint":
                        var dintTag = new Tag<DintPlcMapper, int>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        dintTag.Read();
                        ssReturn = dintTag.Value.ToString();
                        break;

                    case "Lint":
                        var lintTag = new Tag<LintPlcMapper, long>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        lintTag.Read();
                        ssReturn = lintTag.Value.ToString();
                        break;

                    case "Real":
                        var realTag = new Tag<RealPlcMapper, float>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        realTag.Read();
                        ssReturn = realTag.Value.ToString("F").Replace(",", ".");
                        break;

                    case "String":
                        var stringTag = new Tag<StringPlcMapper, string>() { Name = ssNomeTag, Gateway = ssHost, Path = PATH, PlcType = auxTipoPlc, Protocol = Protocol.ab_eip, Timeout = TimeSpan.FromMilliseconds(ssTimeout) };
                        stringTag.Read();
                        ssReturn = stringTag.Value.ToString();
                        break;
                }
            }
            catch (Exception Ex)
            {
                ssStatus = false;
                ssReturn = Ex.ToString();
            }
        } // MssReadCLP
        static void Main(string[] args)
        {
            string ssHost = "192.168.0.10";
            string ssNomeTag = "REAL";
            string ssTipo = "Real";
            string ssTipoPlc = "ControlLogix";
            int ssTimeout = 2500;
            
            Console.WriteLine("oi1");
            MssReadCLP(out bool ssStatus, out string ssReturn, ssHost, ssNomeTag, ssTipo, ssTipoPlc, ssTimeout);
            Console.WriteLine(ssStatus);
            Console.WriteLine(ssReturn);
            Console.WriteLine();

            string ssValor = "0,56934567";
            Console.WriteLine("oi2");
            MssWriteCLP(out bool ssStatus2, out string ssReturn2, ssHost, ssNomeTag, ssTipo, ssTipoPlc, ssTimeout, ssValor);
            Console.WriteLine(float.Parse(ssValor));
            Console.WriteLine(float.Parse(ssValor).ToString("F").Replace(",", "."));
            Console.WriteLine(float.Parse(ssValor).ToString("F3").Replace(",", "."));
            Console.WriteLine(float.Parse(ssValor).ToString("F7").Replace(",", "."));
            Console.WriteLine(float.Parse(ssValor).ToString("F9").Replace(",", "."));
            Console.WriteLine(ssStatus2);
            Console.WriteLine(ssReturn2);

            Console.Read();
        }
    }
}
