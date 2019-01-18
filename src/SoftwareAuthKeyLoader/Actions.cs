using SoftwareAuthKeyLoader.Kmm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader
{
    internal static class Actions
    {
        public static int LoadAuthenticationKey(bool targetSpecificSuId, int wacnId, int systemId, int unitId, byte[] key)
        {
            Output.DebugLine("LoadAuthenticationKey() :: targetSpecificSuId: {0}, wacnId: 0x{1:X}, systemId: 0x{2:X}, unitId: 0x{3:X}, key (hex): {4}", targetSpecificSuId, wacnId, systemId, unitId, BitConverter.ToString(key));

            SuId commandSuId = new SuId(wacnId, systemId, unitId);
            Output.DebugLine("command suid - {0}", commandSuId.ToString());

            LoadAuthenticationKeyCommand commandKmmBody = new LoadAuthenticationKeyCommand(targetSpecificSuId, commandSuId, key);
            Output.DebugLine("command kmm body - {0}", commandKmmBody.ToString());

            KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);
            Output.DebugLine("command kmm frame - {0}", commandKmmFrame.ToString());
            byte[] toRadio = commandKmmFrame.ToBytes();

            byte[] fromRadio;

            try
            {
                fromRadio = Network.QueryRadio(toRadio);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("unable to connect to radio: {0}", ex.Message);
                return -1;
            }

            KmmFrame responseKmmFrame = new KmmFrame(fromRadio);
            Output.DebugLine("response kmm frame - {0}", responseKmmFrame.ToString());

            KmmBody responseKmmBody = responseKmmFrame.KmmBody;

            if (responseKmmBody is LoadAuthenticationKeyResponse)
            {
                Output.DebugLine("received LoadAuthenticationKeyResponse kmm");
                LoadAuthenticationKeyResponse loadAuthenticationKeyResponse = responseKmmBody as LoadAuthenticationKeyResponse;
                Output.DebugLine("response kmm body - {0}", loadAuthenticationKeyResponse.ToString());
                Output.DebugLine("response suid - {0}", loadAuthenticationKeyResponse.SuId.ToString());
                
                if (loadAuthenticationKeyResponse.AssignmentSuccess == true && loadAuthenticationKeyResponse.Status == Status.CommandWasPerformed)
                {
                    return 0;
                }
                else
                {
                    Output.ErrorLine("abnormal response - assignment success: {0}, status: {1} (0x{2:X2})", loadAuthenticationKeyResponse.AssignmentSuccess, loadAuthenticationKeyResponse.Status.ToString(), (byte)loadAuthenticationKeyResponse.Status);
                    return -1;
                }
            }
            else if (responseKmmBody is NegativeAcknowledgement)
            {
                Output.ErrorLine("received NegativeAcknowledgement kmm");
                NegativeAcknowledgement negativeAcknowledgement = responseKmmBody as NegativeAcknowledgement;
                Output.DebugLine("response kmm body - {0}", negativeAcknowledgement.ToString());
                return -1;
            }
            else
            {
                Output.ErrorLine("received unexpected kmm");
                return -1;
            }
        }

        public static int DeleteAuthenticationKey(bool targetSpecificSuId, bool deleteAllKeys, int wacnId, int systemId, int unitId)
        {
            Output.DebugLine("DeleteAuthenticationKey() :: targetSpecificSuId: {0}, deleteAllKeys: {1}, wacnId: 0x{2:X}, systemId: 0x{3:X}, unitId: 0x{4:X}", targetSpecificSuId, deleteAllKeys, wacnId, systemId, unitId);

            SuId commandSuId = new SuId(wacnId, systemId, unitId);
            Output.DebugLine("command suid - {0}", commandSuId.ToString());

            DeleteAuthenticationKeyCommand commandKmmBody = new DeleteAuthenticationKeyCommand(targetSpecificSuId, deleteAllKeys, commandSuId);
            Output.DebugLine("command kmm body - {0}", commandKmmBody.ToString());

            KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);
            Output.DebugLine("command kmm frame - {0}", commandKmmFrame.ToString());
            byte[] toRadio = commandKmmFrame.ToBytes();

            byte[] fromRadio;

            try
            {
                fromRadio = Network.QueryRadio(toRadio);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("unable to connect to radio: {0}", ex.Message);
                return -1;
            }

            KmmFrame responseKmmFrame = new KmmFrame(fromRadio);
            Output.DebugLine("response kmm frame - {0}", responseKmmFrame.ToString());

            KmmBody responseKmmBody = responseKmmFrame.KmmBody;

            if (responseKmmBody is DeleteAuthenticationKeyResponse)
            {
                Output.DebugLine("received DeleteAuthenticationKeyResponse kmm");
                DeleteAuthenticationKeyResponse deleteAuthenticationKeyResponse = responseKmmBody as DeleteAuthenticationKeyResponse;
                Output.DebugLine("response kmm body - {0}", deleteAuthenticationKeyResponse.ToString());
                Output.DebugLine("response suid - {0}", deleteAuthenticationKeyResponse.SuId.ToString());

                if (deleteAuthenticationKeyResponse.Status == Status.CommandWasPerformed)
                {
                    return 0;
                }
                else
                {
                    Output.ErrorLine("abnormal response - status: {0} (0x{1:X2})", deleteAuthenticationKeyResponse.Status.ToString(), (byte)deleteAuthenticationKeyResponse.Status);
                    return -1;
                }
            }
            else if (responseKmmBody is NegativeAcknowledgement)
            {
                Output.ErrorLine("received NegativeAcknowledgement kmm");
                NegativeAcknowledgement negativeAcknowledgement = responseKmmBody as NegativeAcknowledgement;
                Output.DebugLine("response kmm body - {0}", negativeAcknowledgement.ToString());
                return -1;
            }
            else
            {
                Output.ErrorLine("received unexpected kmm");
                return -1;
            }
        }

        public static int ListActiveSuId()
        {
            Output.DebugLine("ListActiveSuId()");

            InventoryCommandListActiveSuId commandKmmBody = new InventoryCommandListActiveSuId();
            Output.DebugLine("command kmm body - {0}", commandKmmBody.ToString());

            KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);
            Output.DebugLine("command kmm frame - {0}", commandKmmFrame.ToString());
            byte[] toRadio = commandKmmFrame.ToBytes();

            byte[] fromRadio;

            try
            {
                fromRadio = Network.QueryRadio(toRadio);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("unable to connect to radio: {0}", ex.Message);
                return -1;
            }

            KmmFrame responseKmmFrame = new KmmFrame(fromRadio);
            Output.DebugLine("response kmm frame - {0}", responseKmmFrame.ToString());

            KmmBody responseKmmBody = responseKmmFrame.KmmBody;

            if (responseKmmBody is InventoryResponseListActiveSuId)
            {
                Output.DebugLine("received InventoryResponseListActiveSuId kmm");
                InventoryResponseListActiveSuId inventoryResponseListActiveSuId = responseKmmBody as InventoryResponseListActiveSuId;
                Output.DebugLine("response kmm body - {0}", inventoryResponseListActiveSuId.ToString());
                Output.DebugLine("response suid - {0}", inventoryResponseListActiveSuId.SuId.ToString());

                if (inventoryResponseListActiveSuId.Status == Status.CommandWasPerformed)
                {
                    Output.InfoLine("WACN: 0x{0:X}, System: 0x{1:X}, Unit: 0x{2:X}, Key Assigned: {3}, Is Active: {4}", inventoryResponseListActiveSuId.SuId.WacnId, inventoryResponseListActiveSuId.SuId.SystemId, inventoryResponseListActiveSuId.SuId.UnitId, inventoryResponseListActiveSuId.KeyAssigned, inventoryResponseListActiveSuId.ActiveSuId);
                    return 0;
                }
                else
                {
                    Output.ErrorLine("abnormal response - status: {0} (0x{1:X2})", inventoryResponseListActiveSuId.Status.ToString(), (byte)inventoryResponseListActiveSuId.Status);
                    return -1;
                }
            }
            else if (responseKmmBody is NegativeAcknowledgement)
            {
                Output.ErrorLine("received NegativeAcknowledgement kmm");
                NegativeAcknowledgement negativeAcknowledgement = responseKmmBody as NegativeAcknowledgement;
                Output.DebugLine("response kmm body - {0}", negativeAcknowledgement.ToString());
                return -1;
            }
            else
            {
                Output.ErrorLine("received unexpected kmm");
                return -1;
            }
        }

        public static int ListSuIdItems()
        {
            Output.DebugLine("ListSuIdItems()");

            bool needsAnotherRun = true;
            int inventoryMarker = 0;

            while (needsAnotherRun)
            {
                InventoryCommandListSuIdItems commandKmmBody = new InventoryCommandListSuIdItems(inventoryMarker, 59);
                Output.DebugLine("command kmm body - {0}", commandKmmBody.ToString());

                KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);
                Output.DebugLine("command kmm frame - {0}", commandKmmFrame.ToString());
                byte[] toRadio = commandKmmFrame.ToBytes();

                byte[] fromRadio;

                try
                {
                    fromRadio = Network.QueryRadio(toRadio);
                }
                catch (Exception ex)
                {
                    Output.ErrorLine("unable to connect to radio: {0}", ex.Message);
                    return -1;
                }

                KmmFrame responseKmmFrame = new KmmFrame(fromRadio);
                Output.DebugLine("response kmm frame - {0}", responseKmmFrame.ToString());

                KmmBody responseKmmBody = responseKmmFrame.KmmBody;

                if (responseKmmBody is InventoryResponseListSuIdItems)
                {
                    Output.DebugLine("received InventoryResponseListSuIdItems kmm");
                    InventoryResponseListSuIdItems inventoryResponseListSuIdItems = responseKmmBody as InventoryResponseListSuIdItems;
                    Output.DebugLine("response kmm body - {0}", inventoryResponseListSuIdItems.ToString());

                    inventoryMarker = inventoryResponseListSuIdItems.InventoryMarker;
                    Output.DebugLine("inventory marker - {0}", inventoryMarker);

                    if (inventoryMarker > 0)
                    {
                        needsAnotherRun = true;
                    }
                    else
                    {
                        needsAnotherRun = false;
                    }

                    foreach (SuIdStatus responseSuIdStatus in inventoryResponseListSuIdItems.SuIdStatuses)
                    {
                        Output.InfoLine("WACN: 0x{0:X}, System: 0x{1:X}, Unit: 0x{2:X}, Key Assigned: {3}, Is Active: {4}", responseSuIdStatus.SuId.WacnId, responseSuIdStatus.SuId.SystemId, responseSuIdStatus.SuId.UnitId, responseSuIdStatus.KeyAssigned, responseSuIdStatus.ActiveSuId);
                    }
                }
                else if (responseKmmBody is NegativeAcknowledgement)
                {
                    Output.ErrorLine("received NegativeAcknowledgement kmm");
                    NegativeAcknowledgement negativeAcknowledgement = responseKmmBody as NegativeAcknowledgement;
                    Output.DebugLine("response kmm body - {0}", negativeAcknowledgement.ToString());
                    return -1;
                }
                else
                {
                    Output.ErrorLine("received unexpected kmm");
                    return -1;
                }
            }

            return 0;
        }
    }
}
