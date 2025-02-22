//SECTION - base data elements
const campaigns = document.getElementById("campaignFilter");
//!SECTION - base data elements

//SECTION - employee filters
const omurFilter = document.getElementById("omurFilter");
const mantagheFilter = document.getElementById("mantagheFilter");
const hozeFilter = document.getElementById("hozeFilter");
const branchFilter = document.getElementById("branchFilter");
const btnAccFilter = document.getElementById("btnFilter");

const currentCampaignId = document.getElementById("currentCampaignId").value;
const currentEmployeeId = document.getElementById("currentEmployeeId").value;
const currentEmployeeRole = document.getElementById("role").value;
const currentEmployeeDepartment = document.getElementById("department").value;
const currentEmployeeRegion = document.getElementById("region").value;
const currentEmployeeArea = document.getElementById("area").value;
const currentEmployeeBranch = document.getElementById("branch").value;

const branchCustomers = document.getElementById("branchCustomers");
const branchEmployee = document.getElementById("branchEmployee");
const directAssignedCustomersPanel = document.getElementById(
  "directAssignedCustomersPanel"
);

const branchCustomerTitle = document.getElementById("branchCustomerTitle");
const branchEmployeeTitle = document.getElementById("branchEmployeeTitle");
const directAssignedCustomerTitle = document.getElementById(
  "directAssignedCustomerTitle"
);

const directAssignedCustomers = document.getElementById(
  "directAssignedCustomers"
);

const pagingDC = document.getElementById("pagingDC");
const pagingC = document.getElementById("pagingC");
const pagingE = document.getElementById("pagingE");

//!SECTION - employee filters

//SECTION - data tabel

const personels = document.getElementById("personels");
const customers = document.getElementById("customers");
const managers = document.getElementById("managers");
const emploees = document.getElementById("emploees");

//!SECTION - data tabel

//SECTION - directCustomersPage

const dcFirst = document.getElementById("dcFirst");
const dcPerviews = document.getElementById("dcPerviews");
const dcNext = document.getElementById("dcNext");
const dcLast = document.getElementById("dcLast");
const dcTotalPages = document.getElementById("dcTotalPages");
const dcCurrentPage = document.getElementById("dcCurrentPage");

//!SECTION - customersPage

//SECTION - customersPage

const cFirst = document.getElementById("cFirst");
const cPerviews = document.getElementById("cPerviews");
const cNext = document.getElementById("cNext");
const cLast = document.getElementById("cLast");
const cTotalPages = document.getElementById("cTotalPages");
const cCurrentPage = document.getElementById("cCurrentPage");

//!SECTION - customersPage

//SECTION - employeePage

const eFirst = document.getElementById("eFirst");
const ePerviews = document.getElementById("ePerviews");
const eNext = document.getElementById("eNext");
const eLast = document.getElementById("eLast");
const eTotalPages = document.getElementById("eTotalPages");
const eCurrentPage = document.getElementById("eCurrentPage");

//!SECTION - employeePage

//SECTION - data url
const campaginsUrl = "/Api/GetAllCampaigns";

//NOTE filter urls
const omurUrl = "/Api/GetOmurList";
const mantagheUrl = "/Api/GetMantagheByOmur/";
const hozeUrl = "/Api/GetHozeByMantaghe/";
const branchUrl = "/Api/GetBranchByHoze/";
const getHozeByBranchUrl = "/Api/GetHozeByBranch/";

//NOTE data urls
const getChildStatsUrl = "/Api/GetChildStats";
const getChildEmployeesUrl = "/Api/GetChildStats";
const getBranchInfoByEmployeeUrl = "/Api/GetBranchInfoByEmployee";
const getBranchInfoByCustomerUrl = "/Api/GetBranchInfoByCustomer";
const getDirectAssignmentsForRole = "/Api/GetDirectAssignmentsForRole";

//!SECTION - data url

//SECTION - validation error
const cerror = "کمپین انتخاب نشده!";
const aError = "مدیر حساب انتخاب نشده!";
//!SECTION - validation error

let unitTypeId = 2;

let dataGetChildStats = {};
let dataGetChildStatsC = {};
let dataGetChildStatsA = {};
let getDirectAssignmentsForRoleParam = {};
var branchIdG;
