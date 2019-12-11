using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using System.Collections;

namespace StasHelper
{
    public class FormHelper
    {
        public delegate void SetChooseFromList();
        public SAPbouiCOM.Application _SBO_Aplication { get; } // указывать readonly необязательно
        public SAPbouiCOM.IForm _Form { get; } // свойство только для чтения
        private Hashtable CFLtoEditTextAlias = new Hashtable();

      
       
        public FormHelper(SAPbouiCOM.Application _SBO_Aplication, SAPbouiCOM.IForm _Form)
        {
            this._SBO_Aplication = _SBO_Aplication;
            this._Form = _Form;
        }

        public SAPbouiCOM.ChooseFromList AddChooseFromList(string ObjectType, string UniqueID, SAPbouiCOM.EditText EditText,  string ChooseFromListAlias = "Code", SetChooseFromList SetCFL = null)
        {
            try
            {
                SAPbouiCOM.ChooseFromListCollection oCFLs;
                // Warning!!! Optional parameters not supported
                SAPbouiCOM.ChooseFromList oCFL;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams;
                SAPbouiCOM.Conditions oCons = (SAPbouiCOM.Conditions)_SBO_Aplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions);

               
                oCFLs = _Form.ChooseFromLists;
                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)_SBO_Aplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);
                //  Adding CFL for itemcode
                oCFLCreationParams.MultiSelection = false;
                oCFLCreationParams.ObjectType = ObjectType;
                oCFLCreationParams.UniqueID = UniqueID;

                oCFL = oCFLs.Add(oCFLCreationParams);

                //SAPbouiCOM.Condition oCon;
                //if (!(ObjectSubType == null))
                //{
                //    oCFL.SetConditions(null);

                //    oCon = oCons.Add();

                //    if ((ObjectSubType.ToString() == "BMT_Route"))
                //    {
                //        oCon.Alias = "Code";
                //        oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_NULL;

                //    }

                //    oCFL.SetConditions(oCons);

                EditText.ChooseFromListUID = UniqueID;
                EditText.ChooseFromListAlias = ChooseFromListAlias;

                if (SetCFL != null)
                {
                    CFLtoEditTextAlias.Add(EditText.Item.UniqueID, SetCFL);
                }

                EditText.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.ChooseFromListOneValue);

                return oCFL;
                }
                       
            catch (System.Exception MsgBox)
            {
                return null;
            }
        }

        public void ChooseFromListOneValue(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.EditText uktzedEditText = ((SAPbouiCOM.EditText)sboObject);

            if (pVal.ActionSuccess)
            {
                SAPbouiCOM.SBOChooseFromListEventArg cfl = ((SAPbouiCOM.SBOChooseFromListEventArg)(pVal));
                if (cfl.SelectedObjects == null)
                {
                    return;
                }
                else
                {
                    var iChoose = ((SAPbouiCOM.SBOChooseFromListEventArg)(pVal));
                    SAPbouiCOM.DataTable dataTable = iChoose.SelectedObjects;

                    if (dataTable == null || dataTable.Rows.Count == 0)
                    {
                        return;
                    }
                 
                    string itemCode = dataTable.GetValue(uktzedEditText.ChooseFromListAlias, 0).ToString();
                    SAPbouiCOM.Form activeForm = Application.SBO_Application.Forms.ActiveForm;
                    this._Form.Select();
                    uktzedEditText.Active = true;

                    try
                    {
                        uktzedEditText.String = itemCode;
                    }
                    catch
                    {

                    }

                    activeForm.Select();
                }

            }

            if (CFLtoEditTextAlias[uktzedEditText.Item.UniqueID] != null)
            {
                SetChooseFromList CFLEditText = (SetChooseFromList)CFLtoEditTextAlias[uktzedEditText.Item.UniqueID];
                CFLEditText();
            }


        }

    }

}
