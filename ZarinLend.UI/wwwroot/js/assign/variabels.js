//SECTION - steps of wizard
const campaignStep = document.getElementById("campaignStep");
const accountStep = document.getElementById("accountStep");
const customerStep = document.getElementById("customerStep");

const campaignPanel = document.getElementById("campaignPanel");
const accountPanel = document.getElementById("accountPanel");
const customerPanel = document.getElementById("customerPanel");
const finalPanel = document.getElementById("finalPanel");
//!SECTION - steps of wizard

//NOTE - alert element
const alertElement = document.getElementById("alert");
const nvs = document.getElementById("nvs");

//SECTION - base data elements
const campaigns = document.getElementById("campaigns");
const accountManagers = document.getElementById("accountManagers");
//!SECTION - base data elements

//SECTION - employee selection
const setadEmployeeFilterPanel = document.getElementById(
  "setadEmployeeFilterPanel"
);
const setadEmployeeFilterResultPanel = document.getElementById(
  "setadEmployeeFilterResultPanel"
);
const setadNotAccess = document.getElementById("setadNotAccess");
const txtSetadEmployeeId = document.getElementById("txtSetadEmployeeId");
const setadEmployeeResult = document.getElementById("setadEmployeeResult");

const btnFindSetadEmployee = document.getElementById("btnFindSetadEmployee");
const btnSelectSetadEmployee = document.getElementById(
  "btnSelectSetadEmployee"
);

const setadEmployeeName = document.getElementById("setadEmployeeName");
const setadEmployeeDepartment = document.getElementById(
  "setadEmployeeDepartment"
);
const setadEmployeeRegion = document.getElementById("setadEmployeeRegion");
// const setadEmployeeArea = document.getElementById("setadEmployeeArea");
const setadEmployeeBranch = document.getElementById("setadEmployeeBranch");

const employeeFilterPanel = document.getElementById("employeeFilterPanel");
const employeeInfo = document.getElementById("employeeInfo");
const selectedEmployee = document.getElementById("selectedEmployee");
const employeeData = document.getElementById("employeeData");
const angleEmp = document.getElementById("angleEmp");
const selectedE = document.getElementById("selectedE");
const angleEmployeeButton = document.getElementById("angleEmployeeButton");
//!SECTION - employee selection

//SECTION - current employee data
const currentEmployeeId = document.getElementById("currentEmployeeId").value;
const currentEmployeeRole = document.getElementById("role").value;
const currentEmployeeDepartment = document.getElementById("department").value;
const currentEmployeeRegion = document.getElementById("region").value;
const currentEmployeeArea = document.getElementById("area").value;
const currentEmployeeBranch = document.getElementById("branch").value;
//!SECTION - current employee data

//SECTION - employee filters
const omurFilter = document.getElementById("omurFilter");
const mantagheFilter = document.getElementById("mantagheFilter");
const hozeFilter = document.getElementById("hozeFilter");
const branchFilter = document.getElementById("branchFilter");
const btnAccFilter = document.getElementById("btnAccFilter");
//!SECTION - employee filters

//SECTION - customer filters
const omurCustomerFilter = document.getElementById("omurCustomerFilter");
const mantagheCustomerFilter = document.getElementById(
  "mantagheCustomerFilter"
);
const hozeCustomerFilter = document.getElementById("hozeCustomerFilter");
const branchCustomerFilter = document.getElementById("branchCustomerFilter");
const btnCustomerFilter = document.getElementById("btnCustomerFilter");
const customerStatus = document.getElementById("customerStatus");

//!SECTION - customer filters

//SECTION - pagination
const employeeTotal = document.getElementById("employeeTotal");
const employeeCurrent = document.getElementById("employeeCurrent");

const employeePagingTop = document.getElementById("employeePagingTop");
const employeePagingBottom = document.getElementById("employeePagingBottom");

const totalPagesT = document.getElementById("totalPagesT");
const currentPageT = document.getElementById("currentPageT");
const totalPagesB = document.getElementById("totalPagesB");
const currentPageB = document.getElementById("currentPageB");

const firstT = document.getElementById("firstT");
const perviewsT = document.getElementById("perviewsT");
const nextT = document.getElementById("nextT");
const lastT = document.getElementById("lastT");

const firstB = document.getElementById("firstB");
const perviewsB = document.getElementById("perviewsB");
const nextB = document.getElementById("nextB");
const lastB = document.getElementById("lastB");
//!SECTION - pagination

//SECTION - customer pagination

const customerTotal = document.getElementById("customerTotal");
const customerCurrent = document.getElementById("customerCurrent");

const customerPagingTop = document.getElementById("customerPagingTop");
const customerPagingBottom = document.getElementById("customerPagingBottom");

const cTotalPagesT = document.getElementById("cTotalPagesT");
const cCurrentPageT = document.getElementById("cCurrentPageT");
const cTotalPagesB = document.getElementById("cTotalPagesB");
const cCurrentPageB = document.getElementById("cCurrentPageB");

const cFirstT = document.getElementById("cFirstT");
const cPerviewsT = document.getElementById("cPerviewsT");
const cNextT = document.getElementById("cNextT");
const cLastT = document.getElementById("cLastT");

const cFirstB = document.getElementById("cFirstB");
const cPerviewsB = document.getElementById("cPerviewsB");
const cNextB = document.getElementById("cNextB");
const cLastB = document.getElementById("cLastB");

//!SECTION - customer pagination

//SECTION - data tabel
const personels = document.getElementById("personels");
const customers = document.getElementById("customers");

const rowCount = document.getElementById("rowCount");
//!SECTION - data tabel

let selectedCustomers = document.getElementById("selectedCustomers");
let employeeSelected = document.getElementById("employeeSelected");
let selectedCampaign = document.getElementById("selectedCampaign");

//SECTION - data url
const accountManagersUrl = "/Api/GetRoleChild";
const searchEmployeeUrl = "/Api/SearchEmployee";
const campaginsUrl = "/Api/GetAllCampaigns";

//NOTE filter urls
const omurUrl = "/Api/GetOmurList";
const mantagheUrl = "/Api/GetMantagheByOmur/";
const hozeUrl = "/Api/GetHozeByMantaghe/";
const branchUrl = "/Api/GetBranchByHoze/";
const getHozeByBranchUrl = "/Api/GetHozeByBranch/";

const getChildEmployeesUrl = "/Api/GetChildEmployees";
const getChildAssigneeUrl = "/Api/GetChildAssignee";
const postAssignment = "/Api/Assignment";
const getSetadEmployee = "/Api/GetSetadEmployee";
//!SECTION - data url

//SECTION - validation error
const cerror = "کمپین انتخاب نشده!";
const aError = "مدیر حساب انتخاب نشده!";
//!SECTION - validation error

let pageNumber = 1;

let dataRequestEmployee = {};
let dataRequestCustomer = {};
let dataRequestAssign = {};

let slide = false;
let setError = false;
let customerData = [];
let customerDataInfo = [];
