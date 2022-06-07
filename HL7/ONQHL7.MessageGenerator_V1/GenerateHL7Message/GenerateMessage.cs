using System;
using System.Collections.Generic;
using System.IO;

namespace ONQHL7.MessageGenerator_V1.GenerateHL7Message
{
    public class GenerateMessage
    {
        private GenerateSegmentMSH m_GenerateSegmentMSH = new();
        private GenerateSegmentPID m_GenerateSegmentPID = new();
        //private GenerateSegmentPV1 m_GenerateSegmentPV1 = new();
        private GenerateSegmentORC m_GenerateSegmentORC = new();
        private GenerateSegmentOBR m_GenerateSegmentOBR = new();
        private GenerateSegmentOBX m_GenerateSegmentOBX = new();
        private GenerateSegmentPhessOBX m_GenerateSegmentPhessOBX = new();

        private MessageHeader m_MessageHeader = new();
        private PatientIdentification m_PatientIdentification = new();
        //private PatientVisit m_PatientVisit = new();
        private CommonOrder m_CommonOrder = new();
        private ObservationRequest m_ObservationRequest = new();
        private ObservationResult m_ObservationResult = new();
        private ObservationResult m_ObservationResultPhess = new();
        public string GenerateHL7Message(string a_sMesssageFilesPath, MessageData a_mMessageData)
        {
            m_MessageHeader = a_mMessageData.MessageHeader != null ? a_mMessageData.MessageHeader : null;
            m_PatientIdentification = a_mMessageData.PatientIdentification;
            //m_PatientVisit = a_mMessageData.PatientVisit;
            m_CommonOrder = a_mMessageData.CommonOrder;
            m_ObservationRequest = a_mMessageData.ObservationRequest;
            m_ObservationResult = a_mMessageData.ObservationResult;
            m_ObservationResultPhess = a_mMessageData.ObservationResultPhess;


            try
            {
                string sFileName = $"{ m_MessageHeader.MsgControlId}{ DateTime.UtcNow:HHmmfff}.HL7";
                string sNewFile = $"{a_sMesssageFilesPath}{sFileName}";

                if (!File.Exists(sNewFile))
                {
                    var file = File.Create(sNewFile);
                    file.Close();
                    using (var sr = new StreamWriter(sNewFile, true))
                    {
                        try
                        {
                            //Generate the Message Header segment
                            if (m_MessageHeader != null)
                                sr.WriteLine(m_GenerateSegmentMSH.CreateMSGSegment(m_MessageHeader));
                            else
                                throw new Exception("No data found for MSG");

                            //Generate the Patient Identification segment
                            if (m_PatientIdentification != null)
                            {
                                if (m_PatientIdentification.DateOfBirth!=null)
                                m_PatientIdentification.DateOfBirth = Convert.ToDateTime(m_PatientIdentification.DateOfBirth).ToString();
                                sr.WriteLine(m_GenerateSegmentPID.CreatePIDSegment(m_PatientIdentification));
                            }                              
                            else
                                throw new Exception("No data found for PID");

                            ////Commented out because currently not tracked in QLIMS
                            //////Generate the Patient Visit segment
                            ////if (m_PatientVisit != null)
                            ////	a_sMessage = m_GenerateSegmentPV1.CreatePV1Segment();
                            ////else
                            ////	throw new Exception("No data found for PV1");

                            //Generate the Common Order segment
                            if (m_CommonOrder != null)
                                sr.WriteLine(m_GenerateSegmentORC.CreateORCSegment(m_CommonOrder));
                            else
                                throw new Exception("No data found for ORC");
                            //Generate the Observation Request segment
                            if (m_ObservationRequest != null)
                            {
                                if (m_ObservationRequest.SpecimenReceivedDateAndTime != null)
                                    m_ObservationRequest.SpecimenReceivedDateAndTime = Convert.ToDateTime(m_ObservationRequest.SpecimenReceivedDateAndTime).ToString();
                                sr.WriteLine(m_GenerateSegmentOBR.CreateOBRSegment(m_ObservationRequest));
                            }
                                
                            else
                                throw new Exception("No data found for OBR");

                            //Generate the Observation Result segment
                            if (m_ObservationResult != null)
                                sr.WriteLine(m_GenerateSegmentOBX.CreateOBXSegment(m_ObservationResult));
                            else
                                throw new Exception("No data found for OBX");
                            if (m_ObservationResultPhess != null)
                                sr.WriteLine(m_GenerateSegmentPhessOBX.CreatePhessOBXSegment(m_ObservationResultPhess));
                            else
                                throw new Exception("No data found for OBX2");
                            sr.Close();
                        }
                        catch (Exception ex)
                        {
                            sr.Close();
                            if (File.Exists(sNewFile))
                                File.Delete(sNewFile);
                            throw new Exception(ex.Message, ex.InnerException);
                        }
                    }
                }
                return sFileName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}