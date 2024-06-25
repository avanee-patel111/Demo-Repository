using SOL.Addressbook.Interfaces;
using SOL.Common.Business.Models;
using SOL.Common.Business.Services;
using SOL.ECM.Models;
using SOL.PMS.Interfaces;
using SOL.WorkFlow.Interfaces;
using SOL.WorkFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOL.WorkFlow.Services
{
    public class PayrollService<T> : BaseService<T>, IPayrollService<T>
    {
        IPayrollRepository<T> _repPayroll = null;
        IAddressbookRepository<T> _repAdrbook;
        IApprovalService<T> _srvApproval = null;
        IWorkflowDocumentService<T> _srvWfDocumentService = null;
        IPmsService<T> _srvPms;
        public PayrollService(IPayrollRepository<T> repPayroll, IAddressbookRepository<T> repAdrbook,
            IApprovalService<T> srvApproval, IWorkflowDocumentService<T> srvWfDocumentService, IPmsService<T> srvPms)
            : base(repAdrbook)
        {
            _repPayroll = repPayroll;
            this._repAdrbook = repAdrbook;
            _srvApproval = srvApproval;
            this._srvWfDocumentService = srvWfDocumentService;
            this._srvPms = srvPms;
        }
        public void SaveBookkeepingConfiguration(BookkeepingConfigurationModel bookkeepingConfigurationModel, int userId, ref string errorMessage)
        {
            var payrollCycleId = bookkeepingConfigurationModel.PAYROLL_CYCLE_ID;
            var payrollEndDate = bookkeepingConfigurationModel.PAYROLL_END_DATE;
            var bookkeepingConfiguration = new WF_BK_BOOKKEEPING_CONFIGURATION();
            var companyId = bookkeepingConfigurationModel.COMPANY_ID;
            var bkcId = bookkeepingConfigurationModel.BKC_ID;
            if (bkcId == default(int))
            {
                bookkeepingConfiguration.COMPANY_ID = companyId;
                bookkeepingConfiguration.DATE_OF_ENTRY = DateTime.UtcNow;
                bookkeepingConfiguration.USER_ID = userId;
                bookkeepingConfiguration.DELETED_FLAG = false;
                bookkeepingConfiguration.WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE = SetPayrollPeriods(bookkeepingConfiguration.WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE, payrollEndDate, payrollCycleId.Value, companyId, userId);
            }
            else
            {
                bookkeepingConfiguration = _repPayroll.GetBookkeepingConfiguration(companyId);
                foreach (var operationHour in bookkeepingConfiguration.WF_BK_OPERATION_HOURS)
                {
                    operationHour.OPERATION_HOUR_LABLE_ONE = null;
                    operationHour.OPERATION_HOUR_LABLE_TWO = null;
                    operationHour.DATE_MODIFIED = DateTime.UtcNow;
                    operationHour.USER_MODIFIED = userId;
                    operationHour.DELETED_FLAG = true;

                }
                if (payrollEndDate != bookkeepingConfiguration.PAYROLL_END_DATE || payrollCycleId != bookkeepingConfiguration.PAYROLL_CYCLE_ID)
                {
                    foreach (var operationHour in bookkeepingConfiguration.WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE)
                    {
                        operationHour.DATE_MODIFIED = DateTime.UtcNow;
                        operationHour.USER_MODIFIED = userId;
                        operationHour.DELETED_FLAG = true;
                    }
                    bookkeepingConfiguration.WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE = SetPayrollPeriods(bookkeepingConfiguration.WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE, payrollEndDate, payrollCycleId.Value, companyId, userId);
                }
            }
            bookkeepingConfiguration.COMPANY_LEGAL_NAME = bookkeepingConfigurationModel.COMPANY_LEGAL_NAME;
            bookkeepingConfiguration.DBA_NAME = bookkeepingConfigurationModel.DBA_NAME;
            bookkeepingConfiguration.INDUSTRY_TYPE_ID = bookkeepingConfigurationModel.INDUSTRY_TYPE_ID;
            bookkeepingConfiguration.COMPANY_ENTITY_TYPE_ID = bookkeepingConfigurationModel.COMPANY_ENTITY_TYPE_ID;
            bookkeepingConfiguration.ACCOUNTING_SOFTWARE_ID = bookkeepingConfigurationModel.ACCOUNTING_SOFTWARE_ID;
            bookkeepingConfiguration.POS_SOFTWARE_ID = bookkeepingConfigurationModel.POS_SOFTWARE_ID;
            bookkeepingConfiguration.OT_MULTIPLIRE = bookkeepingConfigurationModel.OT_MULTIPLIRE;
            bookkeepingConfiguration.PAYROLL_CYCLE_ID = bookkeepingConfigurationModel.PAYROLL_CYCLE_ID;
            bookkeepingConfiguration.PAYROLL_DATE = bookkeepingConfigurationModel.PAYROLL_DATE;
            bookkeepingConfiguration.PAYROLL_END_DATE = bookkeepingConfigurationModel.PAYROLL_END_DATE;
            bookkeepingConfiguration.USER_MODIFIED = userId;
            bookkeepingConfiguration.DATE_MODIFIED = DateTime.UtcNow;
            var operationHours = bookkeepingConfigurationModel.OperationHours;
            var bkOperationHours = new List<WF_BK_OPERATION_HOURS>();
            if (operationHours != null)
            {
                foreach (var operationHour in operationHours)
                {
                    var weekDay = operationHour.WEEKDAY;
                    var bookkeepingConfigurationEntity = bookkeepingConfiguration.WF_BK_OPERATION_HOURS.Where(x => x.WEEKDAY == weekDay).FirstOrDefault();
                    if (bookkeepingConfigurationEntity != null)
                    {
                        bookkeepingConfigurationEntity.WEEKDAY = operationHour.WEEKDAY;
                        bookkeepingConfigurationEntity.OPERATION_HOUR_LABLE_ONE = operationHour.OPERATION_HOUR_LABLE_ONE;
                        bookkeepingConfigurationEntity.OPERATION_HOUR_LABLE_TWO = operationHour.OPERATION_HOUR_LABLE_TWO;
                        bookkeepingConfigurationEntity.DATE_MODIFIED = DateTime.UtcNow;
                        bookkeepingConfigurationEntity.USER_MODIFIED = userId;
                        bookkeepingConfigurationEntity.DELETED_FLAG = false;
                    }
                    else
                    {
                        bookkeepingConfiguration.WF_BK_OPERATION_HOURS.Add(new WF_BK_OPERATION_HOURS()
                        {
                            WEEKDAY = operationHour.WEEKDAY,
                            OPERATION_HOUR_LABLE_ONE = operationHour.OPERATION_HOUR_LABLE_ONE,
                            OPERATION_HOUR_LABLE_TWO = operationHour.OPERATION_HOUR_LABLE_TWO,
                            DATE_MODIFIED = DateTime.UtcNow,
                            DATE_OF_ENTRY = DateTime.UtcNow,
                            USER_ID = userId,
                            USER_MODIFIED = userId,
                            DELETED_FLAG = false,
                        });
                    }
                }
            }

            bookkeepingConfiguration.WF_BK_CONFIGURATION_CUSTOM_FIELD = SetCustomField(bookkeepingConfigurationModel.
                CustomMetadataFieldValueModel, bookkeepingConfiguration.WF_BK_CONFIGURATION_CUSTOM_FIELD,
                ref errorMessage, userId, bkcId);
            if (errorMessage.Length > 0)
            {
                return;
            }
            bookkeepingConfiguration.WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE = SetCustomFieldMultiple(bookkeepingConfigurationModel.
              CustomMetadataFieldMultipleModel, bookkeepingConfiguration.WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE,
              ref errorMessage, userId, bkcId);
            if (errorMessage.Length > 0)
            {
                return;
            }
            _repPayroll.SaveBookkeepingConfiguration(bookkeepingConfiguration);
            bookkeepingConfigurationModel.BKC_ID = bookkeepingConfiguration.BKC_ID;
        }

        private List<WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE> SetCustomFieldMultiple(IEnumerable<CustomMetadataFieldMultipleModel> metaDatas,
            ICollection<WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {
                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var customFieldTypeId = metaData.CUSTOM_FIELD_TYPE_ID;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;
                    var id = metaData.ID;
                    var value = metaData.VALUE;
                    WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE documentMetadataEntity;
                    if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                    {
                        documentMetadataEntity = oldCustomeFields.Where(x => x.VALUE == value).FirstOrDefault();
                        if (documentMetadataEntity == null)
                        {
                            documentMetadataEntity = new WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE();
                            documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            documentMetadataEntity.USER_ID = userId;
                            isNew = true;
                        }
                        else
                        {
                            id = documentMetadataEntity.ID;
                        }
                    }
                    else
                    {
                        documentMetadataEntity = new WF_BK_CONFIGURATION_CUSTOM_FIELD_MULTIPLE();
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = customFieldTypeId;
                    documentMetadataEntity.CUSTOM_FIELD_ID = metaData.CUSTOM_FIELD_ID;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;
                    documentMetadataEntity.VALUE = value;
                    if (isUnique == true)
                    {
                        if (metaData.VALUE.HasValue)
                        {
                            var isExisting = _repPayroll.IsExistingMultipleDDLValue(metaData.VALUE.Value, id, bkcId);
                            if (isExisting)
                            {
                                isExist = true;
                                labelList.Add(label);
                            }
                        }
                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                if (isExist == true)
                {
                    var totalCount = labelList.Count;
                    var lable = string.Empty;
                    for (int i = 0; i < 1; i++)
                    {
                        lable += '"' + labelList[i] + '"';
                    }
                    for (int i = 1; i < totalCount - 1; i++)
                    {
                        lable += ", " + '"' + labelList[i] + '"';
                    }
                    if (totalCount != 1)
                    {
                        for (int i = totalCount - 1; i < totalCount; i++)
                        {
                            lable += " and " + '"' + labelList[i] + '"';
                        }
                    }
                    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                }
            }
            return oldCustomeFields.ToList();
        }


        private List<WF_BK_CONFIGURATION_CUSTOM_FIELD> SetCustomField(IEnumerable<CustomMetadataFieldValueModel> metaDatas,
            ICollection<WF_BK_CONFIGURATION_CUSTOM_FIELD> oldCustomeFields,
            ref string errorMessage, int userId, int bkcId)
        {
            if (oldCustomeFields != null)
            {
                foreach (var customField in oldCustomeFields)
                {
                    customField.DATE_MODIFIED = DateTime.UtcNow;
                    customField.USER_MODIFIED = userId;
                    customField.DELETED_FLAG = true;
                    customField.BOOLEAN_VALUE = null;
                    customField.DATETIME_VALUE = null;
                    customField.DDL_VALUE = null;
                    customField.DECIMAL_VALUE = null;
                    customField.INTEGER_VALUE = null;
                    customField.TEXT_AREA_VALUE = null;
                    customField.TEXT_VALUE = null;
                }
            }
            if (metaDatas != null)
            {
                var isExist = false;
                var labelList = new List<string>();
                foreach (var metaData in metaDatas)
                {
                    var isNew = false;
                    var isUnique = metaData.UNIQUE_FIELD;
                    var id = metaData.CUSTOM_FIELD_TYPE_ID;
                    var dataTypeId = Convert.ToByte(metaData.DATA_TYPE_ID);
                    var label = metaData.FIELD_LABEL;

                    WF_BK_CONFIGURATION_CUSTOM_FIELD documentMetadataEntity;
                    if (oldCustomeFields != null && oldCustomeFields.Count() > 0)
                    {
                        documentMetadataEntity = oldCustomeFields.Where(x => x.CUSTOM_FIELD_TYPE_ID == id).FirstOrDefault();
                        if (documentMetadataEntity == null)
                        {
                            documentMetadataEntity = new WF_BK_CONFIGURATION_CUSTOM_FIELD();
                            documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                            documentMetadataEntity.USER_ID = userId;
                            isNew = true;
                        }
                    }
                    else
                    {
                        documentMetadataEntity = new WF_BK_CONFIGURATION_CUSTOM_FIELD();
                        documentMetadataEntity.DATE_OF_ENTRY = DateTime.UtcNow;
                        documentMetadataEntity.USER_ID = userId;
                        isNew = true;
                    }
                    documentMetadataEntity.CUSTOM_FIELD_TYPE_ID = id;
                    documentMetadataEntity.CUSTOM_FIELD_ID = metaData.CUSTOM_FIELD_ID;
                    documentMetadataEntity.DATA_TYPE_ID = dataTypeId;
                    documentMetadataEntity.DATE_MODIFIED = DateTime.UtcNow;
                    documentMetadataEntity.USER_MODIFIED = userId;
                    documentMetadataEntity.DELETED_FLAG = false;

                    switch ((ECMGlobal.MetadataType)dataTypeId)
                    {
                        case ECMGlobal.MetadataType.Boolean:
                            var booleanValue = metaData.BOOLEAN_VALUE;
                            documentMetadataEntity.BOOLEAN_VALUE = booleanValue;
                            break;
                        case ECMGlobal.MetadataType.Integer:
                            var integerValue = metaData.INTEGER_VALUE;
                            documentMetadataEntity.INTEGER_VALUE = integerValue;
                            if (isUnique == true)
                            {
                                if (integerValue.HasValue)
                                {
                                    var isExisting = _repPayroll.IsExistingIntegerValue(integerValue.Value, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }

                            }
                            break;
                        case ECMGlobal.MetadataType.Decimal:
                            var decimalValue = metaData.DECIMAL_VALUE;
                            documentMetadataEntity.DECIMAL_VALUE = decimalValue;
                            if (isUnique == true)
                            {
                                if (decimalValue.HasValue)
                                {
                                    var isExisting = _repPayroll.IsExistingDecimalValue(decimalValue.Value, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DateTime:
                            var dateTimeValue = metaData.DATETIME_VALUE;
                            documentMetadataEntity.DATETIME_VALUE = dateTimeValue;
                            if (isUnique == true)
                            {
                                if (dateTimeValue.HasValue)
                                {
                                    var isExisting = _repPayroll.IsExistingDateTimeValue(dateTimeValue.Value, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.Text:
                            var textValue = metaData.TEXT_VALUE;
                            documentMetadataEntity.TEXT_VALUE = textValue;
                            if (isUnique == true)
                            {
                                if (textValue != null && textValue != "")
                                {
                                    var isExisting = _repPayroll.IsExistingTextValue(textValue, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.TextArea:
                            var textAreaValue = metaData.TEXT_AREA_VALUE;
                            documentMetadataEntity.TEXT_AREA_VALUE = textAreaValue;
                            if (isUnique == true)
                            {
                                if (textAreaValue != null && textAreaValue != "")
                                {
                                    var isExisting = _repPayroll.IsExistingTextAreaValue(textAreaValue, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                        case ECMGlobal.MetadataType.DropDown:
                            var ddlValue = metaData.DDL_VALUE;
                            documentMetadataEntity.DDL_VALUE = ddlValue;
                            if (isUnique == true)
                            {
                                if (ddlValue != null && ddlValue != 0)
                                {
                                    var isExisting = _repPayroll.IsExistingDDLValue(ddlValue.Value, id, bkcId);
                                    if (isExisting)
                                    {
                                        isExist = true;
                                        labelList.Add(label);
                                    }
                                }
                            }
                            break;
                    }
                    if (isNew)
                    {
                        oldCustomeFields.Add(documentMetadataEntity);
                    }
                }
                if (isExist == true)
                {
                    var totalCount = labelList.Count;
                    var lable = string.Empty;
                    for (int i = 0; i < 1; i++)
                    {
                        lable += '"' + labelList[i] + '"';
                    }
                    for (int i = 1; i < totalCount - 1; i++)
                    {
                        lable += ", " + '"' + labelList[i] + '"';
                    }
                    if (totalCount != 1)
                    {
                        for (int i = totalCount - 1; i < totalCount; i++)
                        {
                            lable += " and " + '"' + labelList[i] + '"';
                        }
                    }
                    errorMessage = string.Format("The value specified for the field(s) {0} already exists in metadata of another document.", lable);
                }
            }
            return oldCustomeFields.ToList();
        }


        public BookkeepingConfigurationModel GetBookkeepingConfiguration(int companyId)
        {
            BookkeepingConfigurationModel bookkeepingConfigurationModel = null;
            var bookkeepingConfiguration = _repPayroll.GetBookkeepingConfiguration(companyId);
            if (bookkeepingConfiguration != null)
            {
                bookkeepingConfigurationModel = new BookkeepingConfigurationModel();
                bookkeepingConfigurationModel.BKC_ID = bookkeepingConfiguration.BKC_ID;
                bookkeepingConfigurationModel.COMPANY_ID = bookkeepingConfiguration.COMPANY_ID;
                bookkeepingConfigurationModel.COMPANY_LEGAL_NAME = bookkeepingConfiguration.COMPANY_LEGAL_NAME;
                bookkeepingConfigurationModel.DBA_NAME = bookkeepingConfiguration.DBA_NAME;
                bookkeepingConfigurationModel.INDUSTRY_TYPE_ID = bookkeepingConfiguration.INDUSTRY_TYPE_ID;
                bookkeepingConfigurationModel.COMPANY_ENTITY_TYPE_ID = bookkeepingConfiguration.COMPANY_ENTITY_TYPE_ID;
                bookkeepingConfigurationModel.ACCOUNTING_SOFTWARE_ID = bookkeepingConfiguration.ACCOUNTING_SOFTWARE_ID;
                bookkeepingConfigurationModel.POS_SOFTWARE_ID = bookkeepingConfiguration.POS_SOFTWARE_ID;
                bookkeepingConfigurationModel.OT_MULTIPLIRE = bookkeepingConfiguration.OT_MULTIPLIRE;
                bookkeepingConfigurationModel.PAYROLL_CYCLE_ID = bookkeepingConfiguration.PAYROLL_CYCLE_ID;
                bookkeepingConfigurationModel.PAYROLL_DATE = bookkeepingConfiguration.PAYROLL_DATE;
                bookkeepingConfigurationModel.PAYROLL_END_DATE = bookkeepingConfiguration.PAYROLL_END_DATE;
                var optionalHours = bookkeepingConfiguration.WF_BK_OPERATION_HOURS.Where(x => x.DELETED_FLAG == false
                     && x.BKC_ID == bookkeepingConfiguration.BKC_ID);
                var OperationHoursModel = new List<BookkeepingConfigurationOperationHoursModel>();
                if (optionalHours != null)
                {
                    foreach (var optionalHour in optionalHours)
                    {
                        OperationHoursModel.Add(new BookkeepingConfigurationOperationHoursModel()
                        {
                            OPERATION_HOURS_ID = optionalHour.OPERATION_HOURS_ID,
                            WEEKDAY = optionalHour.WEEKDAY,
                            OPERATION_HOUR_LABLE_ONE = optionalHour.OPERATION_HOUR_LABLE_ONE,
                            OPERATION_HOUR_LABLE_TWO = optionalHour.OPERATION_HOUR_LABLE_TWO,
                        });
                    }
                }
                bookkeepingConfigurationModel.OperationHours = OperationHoursModel;

                var customeFields = bookkeepingConfiguration.WF_BK_CONFIGURATION_CUSTOM_FIELD.Where(x => x.DELETED_FLAG == false
                    && x.BKC_ID == bookkeepingConfiguration.BKC_ID);
                var customMetadataFieldModel = new List<CustomMetadataFieldValueModel>();
                if (customeFields != null)
                {
                    foreach (var customeField in customeFields)
                    {
                        customMetadataFieldModel.Add(new CustomMetadataFieldValueModel()
                        {
                            BKC_ID = customeField.BKC_ID,
                            CUSTOM_FIELD_ID = customeField.CUSTOM_FIELD_ID,
                            CUSTOM_FIELD_TYPE_ID = customeField.CUSTOM_FIELD_TYPE_ID,

                            DATA_TYPE_ID = customeField.DATA_TYPE_ID,
                            DDL_VALUE = customeField.DDL_VALUE,
                            BOOLEAN_VALUE = customeField.BOOLEAN_VALUE,
                            INTEGER_VALUE = customeField.INTEGER_VALUE,
                            DECIMAL_VALUE = customeField.DECIMAL_VALUE,
                            DATETIME_VALUE = customeField.DATETIME_VALUE,
                            TEXT_VALUE = customeField.TEXT_VALUE,
                            TEXT_AREA_VALUE = customeField.TEXT_AREA_VALUE,
                        });
                    }
                }
                bookkeepingConfigurationModel.CustomMetadataFieldValueModel = customMetadataFieldModel;
            }
            return bookkeepingConfigurationModel;
        }

        public void SaveBankDetails(BookkeepingConfigurationBankDetailsModel bankDetailsModel, int userId, ref string errorMessage)
        {
            var bankDetails = new WF_BK_BANK_DETAILS();
            if (bankDetailsModel.BANK_DETAILS_ID == 0)
            {
                bankDetails.DATE_OF_ENTRY = DateTime.UtcNow;
                bankDetails.USER_ID = userId;
                bankDetails.DELETED_FLAG = false;
            }
            else
            {
                bankDetails = _repPayroll.GetBankDetailsById(bankDetailsModel.BANK_DETAILS_ID);
            }
            var isExist = _repPayroll.CheckAccountNumberIsExist(bankDetailsModel.BANK_DETAILS_ID,
                bankDetailsModel.ACCOUN_NUMBER, bankDetailsModel.ACCOUNT_TYPE);
            if (isExist)
            {
                errorMessage = "This account number is already associated with another company.";
                return;
            }
            bankDetails.COMPANY_ID = bankDetailsModel.COMPANY_ID;
            bankDetails.BANK_NAME = bankDetailsModel.BANK_NAME;
            bankDetails.ACCOUN_NUMBER = bankDetailsModel.ACCOUN_NUMBER;
            bankDetails.ACCOUNT_TYPE = bankDetailsModel.ACCOUNT_TYPE;
            bankDetails.USER_MODIFIED = userId;
            bankDetails.DATE_MODIFIED = DateTime.UtcNow;
            _repPayroll.SaveBankDetails(bankDetails);

        }
        public void SaveCreaditCardDetails(BookkeepingConfigurationCreditCardDetailsModel creditCardDetailsModel, int userId, ref string errorMessage)
        {
            var creditCardDetails = new WF_BK_CREDIT_CARD_DETAILS();
            if (creditCardDetailsModel.CREADIT_CARD_DETAILS_ID == 0)
            {
                creditCardDetails.COMPANY_ID = creditCardDetailsModel.COMPANY_ID;
                creditCardDetails.DATE_OF_ENTRY = DateTime.UtcNow;
                creditCardDetails.USER_ID = userId;
                creditCardDetails.DELETED_FLAG = false;
            }
            else
            {
                creditCardDetails = _repPayroll.GetCreaditCardDetailsById(creditCardDetailsModel.CREADIT_CARD_DETAILS_ID);
            }
            var isExist = _repPayroll.CheckCreditCardNumberIsExist(creditCardDetailsModel.CREADIT_CARD_DETAILS_ID,
              creditCardDetailsModel.CARD_NUMBER, creditCardDetailsModel.CARD_TYPE);
            if (isExist)
            {
                errorMessage = "This credit card number is already associated with another company.";
                return;
            }
            creditCardDetails.BANK_NAME = creditCardDetailsModel.BANK_NAME;
            creditCardDetails.CARD_NUMBER = creditCardDetailsModel.CARD_NUMBER;
            creditCardDetails.CARD_TYPE = creditCardDetailsModel.CARD_TYPE;
            creditCardDetails.USER_MODIFIED = userId;
            creditCardDetails.DATE_MODIFIED = DateTime.UtcNow;
            _repPayroll.SaveCreaditCardDetails(creditCardDetails);
        }
        public List<BookkeepingConfigurationCreditCardDetailsModel> GetCreaditCardDetails(int companyId)
        {

            var creditCardDetailsModel = new List<BookkeepingConfigurationCreditCardDetailsModel>();
            var creditCardDetails = _repPayroll.GetCreditCardDetails(companyId);
            if (creditCardDetails != null)
            {
                foreach (var creditCardDetail in creditCardDetails)
                {

                    var cardTypeName = _repPayroll.GetCreditCardTypeNameByTypeId(creditCardDetail.CARD_TYPE);
                    creditCardDetailsModel.Add(new BookkeepingConfigurationCreditCardDetailsModel()
                    {
                        CREADIT_CARD_DETAILS_ID = creditCardDetail.CREADIT_CARD_DETAILS_ID,
                        COMPANY_ID = creditCardDetail.COMPANY_ID,
                        BANK_NAME = creditCardDetail.BANK_NAME,
                        CARD_TYPE = creditCardDetail.CARD_TYPE,
                        CARD_NUMBER = creditCardDetail.CARD_NUMBER,
                        CARD_TYPE_NAME = cardTypeName,
                    });
                }
            }
            return creditCardDetailsModel;
        }
        public List<BookkeepingConfigurationBankDetailsModel> GetBankDetails(int companyId)
        {
            var bankDetailsModel = new List<BookkeepingConfigurationBankDetailsModel>();
            var bankDetails = _repPayroll.GetBankDetails(companyId);

            if (bankDetails != null)
            {
                foreach (var bank in bankDetails)
                {
                    var bankAccounTypeName = _repPayroll.GetBankAccountTypeNameByAccTypeId(bank.ACCOUNT_TYPE);
                    bankDetailsModel.Add(new BookkeepingConfigurationBankDetailsModel()
                    {
                        BANK_DETAILS_ID = bank.BANK_DETAILS_ID,
                        COMPANY_ID = bank.COMPANY_ID,
                        BANK_NAME = bank.BANK_NAME,
                        ACCOUN_NUMBER = bank.ACCOUN_NUMBER,
                        ACCOUNT_TYPE = bank.ACCOUNT_TYPE,
                        ACCOUNT_TYPE_NAME = bankAccounTypeName,
                    });
                }
            }
            return bankDetailsModel;
        }

        public BookkeepingConfigurationCreditCardDetailsModel GetCreaditCardDetailsById(int creaditCardDetailsId)
        {

            var creditCardDetailsModel = new BookkeepingConfigurationCreditCardDetailsModel();
            var creditCardDetails = _repPayroll.GetCreaditCardDetailsById(creaditCardDetailsId);
            if (creditCardDetails != null)
            {
                creditCardDetailsModel.CREADIT_CARD_DETAILS_ID = creditCardDetails.CREADIT_CARD_DETAILS_ID;
                creditCardDetailsModel.BANK_NAME = creditCardDetails.BANK_NAME;
                creditCardDetailsModel.CARD_NUMBER = creditCardDetails.CARD_NUMBER;
                creditCardDetailsModel.CARD_TYPE = creditCardDetails.CARD_TYPE;
            }
            return creditCardDetailsModel;
        }
        public BookkeepingConfigurationBankDetailsModel GetBankDetailsById(int bankDetailsId)
        {
            var bankDetailsModel = new BookkeepingConfigurationBankDetailsModel();
            var bankDetail = _repPayroll.GetBankDetailsById(bankDetailsId);
            if (bankDetail != null)
            {
                bankDetailsModel.BANK_DETAILS_ID = bankDetail.BANK_DETAILS_ID;
                bankDetailsModel.COMPANY_ID = bankDetail.COMPANY_ID;
                bankDetailsModel.BANK_NAME = bankDetail.BANK_NAME;
                bankDetailsModel.ACCOUN_NUMBER = bankDetail.ACCOUN_NUMBER;
                bankDetailsModel.ACCOUNT_TYPE = bankDetail.ACCOUNT_TYPE;
            }
            return bankDetailsModel;
        }

        public bool DeleteCommon(T entityTypeid, T id, T UserId, out string strMessage)
        {
            if (base.DeleteCommonEntity(entityTypeid, id, UserId, out strMessage))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveSalesAndRevinue(salesAndRevenueModel kpisAndSalesDetails, int userId)
        {
            var companyId = kpisAndSalesDetails.CompanyId;
            var salesCategoryDetails = _repPayroll.GetSalesCategoryDetails(companyId);
            var kpisDetails = _repPayroll.GetKpisDetails(companyId);
            var kpisData = new List<WF_BK_KPIS>();
            var salesCategories = new List<WF_BK_SALES_CATEGORY>();

            if (kpisDetails.Count != default(int))
            {
                foreach (var kpis in kpisDetails)
                {
                    kpis.DATE_MODIFIED = DateTime.UtcNow;
                    kpis.USER_MODIFIED = userId;
                    kpis.DELETED_FLAG = true;
                }
            }

            if (salesCategoryDetails.Count != default(int))
            {
                foreach (var salesCategory in salesCategoryDetails)
                {
                    salesCategory.DATE_MODIFIED = DateTime.UtcNow;
                    salesCategory.USER_MODIFIED = userId;
                    salesCategory.DELETED_FLAG = true;
                }
            }

            setKpisDetails(kpisAndSalesDetails, userId, kpisDetails, kpisData);
            setSalesCategoryDetails(kpisAndSalesDetails, userId, salesCategoryDetails, salesCategories);

            _repPayroll.SaveSalesAndRevinue(kpisData, salesCategories);
        }

        private static void setSalesCategoryDetails(salesAndRevenueModel kpisAndSalesDetails, int userId, List<WF_BK_SALES_CATEGORY> salesCategoryDetails, List<WF_BK_SALES_CATEGORY> salesCategories)
        {
            foreach (var SalesCategoryId in kpisAndSalesDetails.SalesCategoryIds)
            {
                var salesCategory = salesCategoryDetails.Where(x => x.SALES_CATEGORY_ID == SalesCategoryId).FirstOrDefault();
                if (salesCategory != null)
                {
                    salesCategory.DATE_MODIFIED = DateTime.UtcNow;
                    salesCategory.USER_MODIFIED = userId;
                    salesCategory.DELETED_FLAG = false;
                }
                else
                {
                    var salesCategoryData = new WF_BK_SALES_CATEGORY();
                    salesCategoryData.SALES_CATEGORY_ID = SalesCategoryId;
                    salesCategoryData.INDUSTRY_ID = kpisAndSalesDetails.IndustryId;
                    salesCategoryData.COMPANY_ID = kpisAndSalesDetails.CompanyId;
                    salesCategoryData.DATE_MODIFIED = DateTime.UtcNow;
                    salesCategoryData.DATE_OF_ENTRY = DateTime.UtcNow;
                    salesCategoryData.USER_ID = userId;
                    salesCategoryData.USER_MODIFIED = userId;
                    salesCategoryData.DELETED_FLAG = false;
                    salesCategories.Add(salesCategoryData);
                }
            }
        }

        private static void setKpisDetails(salesAndRevenueModel kpisAndSalesDetails, int userId, List<WF_BK_KPIS> kpisDetails, List<WF_BK_KPIS> kpisData)
        {
            foreach (var kpisId in kpisAndSalesDetails.KpisIds)
            {
                var kpisDetail = kpisDetails.Where(x => x.KPIS_ID == kpisId).FirstOrDefault();
                if (kpisDetail != null)
                {
                    kpisDetail.DATE_MODIFIED = DateTime.UtcNow;
                    kpisDetail.USER_MODIFIED = userId;
                    kpisDetail.DELETED_FLAG = false;
                }
                else
                {
                    var kpis = new WF_BK_KPIS();
                    kpis.KPIS_ID = kpisId;
                    kpis.INDUSTRY_ID = kpisAndSalesDetails.IndustryId;
                    kpis.COMPANY_ID = kpisAndSalesDetails.CompanyId;
                    kpis.DATE_MODIFIED = DateTime.UtcNow;
                    kpis.DATE_OF_ENTRY = DateTime.UtcNow;
                    kpis.USER_ID = userId;
                    kpis.USER_MODIFIED = userId;
                    kpis.DELETED_FLAG = false;
                    kpisData.Add(kpis);
                }
            }
        }

        public IdValueModel GetBookKeepingIndustryName(int companyId)
        {
            var industryIdAndName = _repPayroll.GetBookKeepingIndustryName(companyId);
            return industryIdAndName;
        }

        public void SaveNewSalesCategory(string SalescategoryName, int userId, ref string errorMessage, int industryId)
        {

            var isnewSalesCategoryExist = _repPayroll.IsNewSalesCategoryExist(SalescategoryName);
            if (isnewSalesCategoryExist == true)
            {
                errorMessage = "Given sales category name already exist.";
                return;
            }

            var newCategory = new WF_BK_MS_SALES_CATEGORY();
            newCategory.INDUSTRY_ID = industryId;
            newCategory.CATEGORY_NAME = SalescategoryName;
            newCategory.DATE_MODIFIED = DateTime.UtcNow;
            newCategory.DATE_OF_ENTRY = DateTime.UtcNow;
            newCategory.USER_ID = userId;
            newCategory.USER_MODIFIED = userId;
            newCategory.ACTIVE_FLAG = true;
            newCategory.DELETED_FLAG = false;
            _repPayroll.SaveNewSalesCategory(newCategory, SalescategoryName);
        }

        public void SaveClientsAndVendor(CientsAndVendorModel UpadateClientsAndVendor, int userId, ref string errorMessage)
        {
            var clientsAndVendor = new WF_BK_CUSTOMER();
            var checkNameAndType = true;
            var clientVendorName = string.Empty;
            var companyId = UpadateClientsAndVendor.COMPANY_ID;
            if (UpadateClientsAndVendor.CUSTOMER_ID != 0)
            {
                clientsAndVendor = _repPayroll.GetExistingCustomerInfoByCustomerId(UpadateClientsAndVendor.CUSTOMER_ID);
                var isEqual = clientsAndVendor.NAME.ToUpper().Equals(UpadateClientsAndVendor.NAME.ToUpper());
                if (isEqual)
                {
                    checkNameAndType = false;
                }
                else
                {
                    //companyId = clientsAndVendor.COMPANY_ID;
                    checkNameAndType = true;
                }
            }
            if (checkNameAndType)
            {
                clientVendorName = UpadateClientsAndVendor.NAME;
                var nameAndType = _repPayroll.IsClientOrVendorNameAndTypeExist(clientVendorName, UpadateClientsAndVendor.TYPE_ID, companyId);
                if (nameAndType)
                {
                    var typeName = _repPayroll.GetTypeNameByTypeId(UpadateClientsAndVendor.TYPE_ID);
                    errorMessage = "This " + typeName + " already exist.";
                    return;
                }
            }

            clientsAndVendor.NAME = UpadateClientsAndVendor.NAME;
            clientsAndVendor.PHONE_NO = UpadateClientsAndVendor.PHONE_NO;
            clientsAndVendor.STATE_ID = UpadateClientsAndVendor.STATE_ID;
            clientsAndVendor.ADDRESS1 = UpadateClientsAndVendor.ADDRESS1;
            clientsAndVendor.ADDRESS2 = UpadateClientsAndVendor.ADDRESS2;
            clientsAndVendor.CITY = UpadateClientsAndVendor.CITY;
            clientsAndVendor.COUNTRY_ID = UpadateClientsAndVendor.COUNTRY_ID;
            clientsAndVendor.COMPANY_ID = UpadateClientsAndVendor.COMPANY_ID;
            clientsAndVendor.TYPE_ID = UpadateClientsAndVendor.TYPE_ID;
            clientsAndVendor.EMAIL = UpadateClientsAndVendor.EMAIL;
            clientsAndVendor.ZIP = UpadateClientsAndVendor.ZIP;
            clientsAndVendor.URL = UpadateClientsAndVendor.URL;
            clientsAndVendor.VENDOR_1099 = UpadateClientsAndVendor.VENDOR_1099;
            clientsAndVendor.TAX_ID = UpadateClientsAndVendor.TAX_ID;
            clientsAndVendor.DATE_MODIFIED = DateTime.UtcNow;
            clientsAndVendor.DATE_OF_ENTRY = DateTime.UtcNow;
            clientsAndVendor.USER_ID = userId;
            clientsAndVendor.USER_MODIFIED = userId;
            clientsAndVendor.DELETED_FLAG = false;
            _repPayroll.SaveClientsAndVendor(clientsAndVendor);

        }

        public void SaveBookkeepingEmployeeDetails(BookkeepingConfigurationEmployeeModel
            bookkeepingConfigurationEmployeeModel, int userId, ref List<int> employeeAddressIds)
        {
            var employee = new WF_BK_EMPLOYEE();
            var employeeId = bookkeepingConfigurationEmployeeModel.EMPLOYEE_ID;
            if (employeeId == 0)
            {
                employee.DATE_OF_ENTRY = DateTime.UtcNow;
                employee.USER_ID = userId;
                employee.DELETED_FLAG = false;
            }
            else
            {
                employee = _repPayroll.GetBookkeepingEmployeeDetails(employeeId);
                foreach (var employeeSchedule in employee.WF_BK_EMPLOYEE_SCHEDULE)
                {
                    employeeSchedule.DATE_MODIFIED = DateTime.UtcNow;
                    employeeSchedule.USER_MODIFIED = userId;
                    employeeSchedule.DELETED_FLAG = true;
                }
            }
            employee.PRIMARY_DESIGNATION = bookkeepingConfigurationEmployeeModel.PRIMARY_DESIGNATION;
            employee.COMPANY_ID = bookkeepingConfigurationEmployeeModel.COMPANY_ID;
            employee.FIRST_NAME = bookkeepingConfigurationEmployeeModel.FIRST_NAME;
            employee.LAST_NAME = bookkeepingConfigurationEmployeeModel.LAST_NAME;
            employee.MOBILE_NUMBE = bookkeepingConfigurationEmployeeModel.MOBILE_NUMBE;
            employee.USER_MODIFIED = userId;
            employee.DATE_MODIFIED = DateTime.UtcNow;
            var employeeAddressModel = bookkeepingConfigurationEmployeeModel.EmployeeAddressModel;
            if (employeeAddressModel != null)
            {
                foreach (var employeeAddress in employeeAddressModel)
                {
                    var employeeAddressId = employeeAddress.EMPLOYEE_ADDRESS_ID;
                    var employeeAddressEntity = employee.WF_BK_EMPLOYEE_ADDRESS.Where(x => x.EMPLOYEE_ADDRESS_ID == employeeAddressId).FirstOrDefault();
                    if (employeeAddressEntity != null)
                    {
                        employeeAddressEntity.ADDRESS1 = employeeAddress.ADDRESS1;
                        employeeAddressEntity.ADDRESS2 = employeeAddress.ADDRESS2;
                        employeeAddressEntity.CITY = employeeAddress.CITY;
                        employeeAddressEntity.ZIP = employeeAddress.ZIP;
                        employeeAddressEntity.STATE_ID = employeeAddress.STATE_ID;
                        employeeAddressEntity.COUNTRY_ID = employeeAddress.COUNTRY_ID;
                        employeeAddressEntity.DATE_MODIFIED = DateTime.UtcNow;
                        employeeAddressEntity.USER_MODIFIED = userId;
                        employeeAddressEntity.DELETED_FLAG = false;
                    }
                    else
                    {
                        employee.WF_BK_EMPLOYEE_ADDRESS.Add(new WF_BK_EMPLOYEE_ADDRESS()
                        {
                            ADDRESS1 = employeeAddress.ADDRESS1,
                            ADDRESS2 = employeeAddress.ADDRESS2,
                            CITY = employeeAddress.CITY,
                            ZIP = employeeAddress.ZIP,
                            STATE_ID = employeeAddress.STATE_ID,
                            COUNTRY_ID = employeeAddress.COUNTRY_ID,
                            DATE_MODIFIED = DateTime.UtcNow,
                            DATE_OF_ENTRY = DateTime.UtcNow,
                            USER_ID = userId,
                            USER_MODIFIED = userId,
                            DELETED_FLAG = false,
                        });
                    }
                }
            }
            var employeeScheduleModel = bookkeepingConfigurationEmployeeModel.EmployeeScheduleModel;
            if (employeeScheduleModel != null)
            {
                foreach (var employeeSchedule in employeeScheduleModel)
                {
                    var scheduleId = employeeSchedule.SCHEDULE_ID;
                    var weekDayId = employeeSchedule.WEEK_DAY_ID;
                    var employeeScheduleEntity = employee.WF_BK_EMPLOYEE_SCHEDULE.Where(x => x.WEEK_DAY_ID == weekDayId).FirstOrDefault();
                    if (employeeScheduleEntity != null)
                    {
                        employeeScheduleEntity.IN_TIME = employeeSchedule.IN_TIME;
                        employeeScheduleEntity.OUT_TIME = employeeSchedule.OUT_TIME;
                        employeeScheduleEntity.IS_OFF_DAY = employeeSchedule.IS_OFF_DAY;
                        employeeScheduleEntity.WEEK_DAY_ID = employeeSchedule.WEEK_DAY_ID;
                        employeeScheduleEntity.DATE_MODIFIED = DateTime.UtcNow;
                        employeeScheduleEntity.USER_MODIFIED = userId;
                        employeeScheduleEntity.DELETED_FLAG = false;
                    }
                    else
                    {
                        employee.WF_BK_EMPLOYEE_SCHEDULE.Add(new WF_BK_EMPLOYEE_SCHEDULE()
                        {
                            IN_TIME = employeeSchedule.IN_TIME,
                            OUT_TIME = employeeSchedule.OUT_TIME,
                            IS_OFF_DAY = employeeSchedule.IS_OFF_DAY,
                            WEEK_DAY_ID = employeeSchedule.WEEK_DAY_ID,
                            DATE_MODIFIED = DateTime.UtcNow,
                            DATE_OF_ENTRY = DateTime.UtcNow,
                            USER_ID = userId,
                            USER_MODIFIED = userId,
                            DELETED_FLAG = false,
                        });
                    }
                }
            }
            _repPayroll.SaveBookkeepingEmployeeDetails(employee);
            bookkeepingConfigurationEmployeeModel.EMPLOYEE_ID = employee.EMPLOYEE_ID;
            if (employee.WF_BK_EMPLOYEE_ADDRESS.Count > 0)
            {
                foreach (var address in (employee.WF_BK_EMPLOYEE_ADDRESS.Where(x => x.EMPLOYEE_ID ==
                    employee.EMPLOYEE_ID && x.DELETED_FLAG == false).ToList()))
                {
                    employeeAddressIds.Add(address.EMPLOYEE_ADDRESS_ID);
                }
            }
        }

        public int SaveEmployeeDesignation(BookkeepingConfigurationEmployeeDesignationModel employeeDesignationModel,
            int userId, ref string errorMessage)
        {
            var employeeDesignation = new WF_BK_EMPLOYEE_DESIGNATION();
            var employeeDesignationId = employeeDesignationModel.EMPLOYEE_DESIGNATION_ID;
            var payTypeId = employeeDesignationModel.PAY_TYPE_ID;
            var employeeId = employeeDesignationModel.EMPLOYEE_ID;
            if (employeeDesignationId == 0)
            {
                employeeDesignation.DATE_OF_ENTRY = DateTime.UtcNow;
                employeeDesignation.USER_ID = userId;
                employeeDesignation.DELETED_FLAG = false;
            }
            else
            {
                employeeDesignation = _repPayroll.GetEmployeeDesignationDetailsById(employeeDesignationId);
            }
            var startDate = employeeDesignationModel.START_DATE;
            var endDate = employeeDesignationModel.END_DATE;
            var isExist = _repPayroll.IsMultiplierExist(employeeId, startDate, endDate, payTypeId, employeeDesignationId);
            if (isExist)
            {
                errorMessage = "Entry for Start Date and/or End Date is overlapped for the employee.";
                return 0;
            }
            employeeDesignation.EMPLOYEE_ID = employeeId;
            employeeDesignation.DESIGNATION_ID = employeeDesignationModel.DESIGNATION_ID;
            employeeDesignation.PAY_TYPE_ID = employeeDesignationModel.PAY_TYPE_ID;
            employeeDesignation.START_DATE = startDate;
            employeeDesignation.END_DATE = endDate;
            employeeDesignation.RATE = employeeDesignationModel.RATE;
            employeeDesignation.CYCLE_ID = employeeDesignationModel.CYCLE_ID;
            employeeDesignation.USER_MODIFIED = userId;
            employeeDesignation.DATE_MODIFIED = DateTime.UtcNow;
            _repPayroll.SaveBookkeepingEmployeeDesignation(employeeDesignation);
            return employeeDesignationModel.DESIGNATION_ID;
        }


        public void SaveChartOfAccount(ChartOfAccount chartOfAccountModel, int userId)
        {
            WF_BK_CHART_OF_ACCOUNT chartofAccount = null;
            var chartofAccounts = new List<WF_BK_CHART_OF_ACCOUNT>();
            var chartAccountId = chartOfAccountModel.CHART_ACCOUNT_ID;
            var accountName = chartOfAccountModel.ACCOUNT_NAME;
            var accountIds = chartOfAccountModel.ACCOUNT_IDS;
            var comapanyId = chartOfAccountModel.COMPANY_ID;
            var isExisting = chartOfAccountModel.IsExisting;
            if (chartAccountId == 0)
            {
                if (isExisting)
                {
                    if (accountIds == null) { return; }
                    foreach (var accountId in accountIds)
                    {
                        chartofAccount = new WF_BK_CHART_OF_ACCOUNT();
                        chartofAccount.COMPANY_ID = comapanyId;
                        chartofAccount.DATE_MODIFIED = DateTime.UtcNow;
                        chartofAccount.DATE_OF_ENTRY = DateTime.UtcNow;
                        chartofAccount.USER_ID = userId;
                        chartofAccount.USER_MODIFIED = userId;
                        chartofAccount.DELETED_FLAG = false;
                        chartofAccount.ACCOUNT_NAME = _repPayroll.GetAccountName(accountId);
                        chartofAccounts.Add(chartofAccount);
                    }
                }
                else
                {
                    chartofAccount = new WF_BK_CHART_OF_ACCOUNT();
                    chartofAccount.COMPANY_ID = comapanyId;
                    chartofAccount.DATE_MODIFIED = DateTime.UtcNow;
                    chartofAccount.DATE_OF_ENTRY = DateTime.UtcNow;
                    chartofAccount.USER_ID = userId;
                    chartofAccount.USER_MODIFIED = userId;
                    chartofAccount.DELETED_FLAG = false;
                    chartofAccount.ACCOUNT_NAME = accountName;
                    chartofAccounts.Add(chartofAccount);
                }
                _repPayroll.SaveChartOfAccount(chartofAccounts);
            }
            else
            {
                if (accountName == null) { return; }
                chartofAccount = _repPayroll.GetChartOfAccountDetails(chartAccountId);
                chartofAccount.DATE_MODIFIED = DateTime.UtcNow;
                chartofAccount.USER_MODIFIED = userId;
                chartofAccount.ACCOUNT_NAME = accountName;
                _repPayroll.SaveChanges();
            }
        }

        public PayrollPeriod GetPayrollPeriod(int companyId)
        {
            return _repPayroll.GetPayrollPeriod(companyId);
        }

        private ICollection<WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE> SetPayrollPeriods(ICollection<WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE> payrollPeriods, DateTime selectedDate, int ratePeriodId, int companyId, int userId)
        {
            var yearEndDay = GetCompanySetup<int>("YEAR_END_DAY");
            var yearEndMonth = GetCompanySetup<int>("YEAR_END_MONTH");
            var yearStartDay = GetCompanySetup<int>("YEAR_START_DAY");
            var yearStartMonth = GetCompanySetup<int>("YEAR_START_MONTH");            
            var payrollStartDate = selectedDate.AddDays(1);
            var payrollFinalStartDate = selectedDate;           
            var currentYear = selectedDate.Year;
            for (int j = 0; j < 25; j++)
            {
                currentYear = payrollFinalStartDate.Year;
                DateTime firstYearDate = new DateTime(currentYear, yearStartMonth, yearStartDay);
                DateTime lastYearDate = new DateTime(currentYear, yearEndMonth, yearEndDay);
                if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Weekly)
                {
                    for (int i = 0; payrollStartDate <= lastYearDate; i++)
                    {
                                                 
                            payrollStartDate = payrollStartDate.AddDays(7);
                                             
                        //payrollFinalStartDate = payrollStartDate.AddDays(1);
                    }                   
                    //payrollFinalStartDate = payrollStartDate.AddDays(1);
                    payrollFinalStartDate = payrollStartDate;
                }
                else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Bi_Weekly)
                {
                    for (int i = 0; payrollStartDate <= lastYearDate; i++)
                    {
                       
                            payrollStartDate = payrollStartDate.AddDays(14);
                        
                        //payrollFinalStartDate = payrollStartDate.AddDays(1);
                    }
                    //payrollFinalStartDate = payrollStartDate.AddDays(1);
                }
                else if (ratePeriodId == (int)WorkflowGloble.PayrollPeriodType.Monthly)
                {
                    for (int i = 0; payrollStartDate < lastYearDate; i++)
                    {
                        var nextMonth = new DateTime(payrollStartDate.AddMonths(1).Year, payrollStartDate.AddMonths(1).Month, 1);
                        var year = payrollStartDate.Year;
                        var month = payrollStartDate.Month;
                        int days = System.DateTime.DaysInMonth(currentYear, month);
                        payrollStartDate = payrollStartDate.AddDays(days).AddDays(-1);
                        payrollStartDate = payrollStartDate.AddDays(1);
                        payrollFinalStartDate = payrollStartDate;
                    }
                }
                var payrollPeriod = new WF_BK_CONFIGURATION_PAYROLL_YEAR_START_DATE();
                payrollPeriod.COMPANY_ID = companyId;
                payrollPeriod.PAYROLL_CYCLE_ID = ratePeriodId;
                payrollPeriod.PAYROLL_START_DATE = payrollFinalStartDate;
                payrollPeriod.YEAR = payrollFinalStartDate.Year;
                payrollPeriod.DATE_MODIFIED = DateTime.UtcNow;
                payrollPeriod.DATE_OF_ENTRY = DateTime.UtcNow;
                payrollPeriod.USER_ID = userId;
                payrollPeriod.USER_MODIFIED = userId;
                payrollPeriod.DELETED_FLAG = false;
                payrollPeriods.Add(payrollPeriod);
            }
            return payrollPeriods;
        }


        public DateTime GetPayrollYearStartDate(int year, int companyId)
        {
            return _repPayroll.GetPayrollYearStartDate(year, companyId);
        }
        public void SaveChartofAccountNames(WF_MS_ACCOUNT objMsAccount, int userId, ref string errorMessage)
        {
            var is_validate = _repPayroll.ValidateChartofAccountNames(objMsAccount.ACCOUNT_ID, objMsAccount.ACCOUNT_NAME);

            if (is_validate == true)
            {
                errorMessage = "Account name already exists";
                return;
            }

            if (objMsAccount.ACCOUNT_ID == default(int))
            {
                objMsAccount.USER_ID = userId;
                objMsAccount.USER_MODIFIED = userId;
                objMsAccount.DATE_OF_ENTRY = DateTime.UtcNow;
                objMsAccount.DATE_MODIFIED = DateTime.UtcNow;
                objMsAccount.ACTIVE_FLAG = true;
                objMsAccount.DELETED_FLAG = false;

            }
            else
            {
                objMsAccount.USER_MODIFIED = userId;
                objMsAccount.DATE_MODIFIED = DateTime.UtcNow;
            }
            _repPayroll.SaveChartofAccountNames(objMsAccount);
        }

        public void SaveIndustryWiseDesignations(WF_BK_MS_DESIGNATION objMSDesignation, int userId, ref string errorMessage)
        {
            var is_validate = _repPayroll.ValidateIndustryWiseDesignations(objMSDesignation.INDUSTRY_ID, objMSDesignation.DESIGNATION_NAME);

            if (is_validate == true)
            {
                errorMessage = "Designation name already exists";
                return;
            }

            if (objMSDesignation.DESIGNATION_ID == default(int))
            {
                objMSDesignation.USER_ID = userId;
                objMSDesignation.USER_MODIFIED = userId;
                objMSDesignation.DATE_OF_ENTRY = DateTime.UtcNow;
                objMSDesignation.DATE_MODIFIED = DateTime.UtcNow;
                objMSDesignation.ACTIVE_FLAG = true;
                objMSDesignation.DELETED_FLAG = false;

            }
            else
            {
                objMSDesignation.USER_MODIFIED = userId;
                objMSDesignation.DATE_MODIFIED = DateTime.UtcNow;
            }
            _repPayroll.SaveIndustryWiseDesignations(objMSDesignation);
        }

    }
}
