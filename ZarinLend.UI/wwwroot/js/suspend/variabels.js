//SECTION - customer filters
const omurCustomerFilter = document.getElementById("omurCustomerFilter");
const mantagheCustomerFilter = document.getElementById(
  "mantagheCustomerFilter"
);
const hozeCustomerFilter = document.getElementById("hozeCustomerFilter");
const branchCustomerFilter = document.getElementById("branchCustomerFilter");
const btnCustomerFilter = document.getElementById("btnCustomerFilter");
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
//!SECTION - data tabel

let selectedCustomers = document.getElementById("selectedCustomers");
let employeeSelected = document.getElementById("employeeSelected");
let selectedCampaign = document.getElementById("selectedCampaign");

//SECTION - data url
const accountManagersUrl = "/Api/GetRoleChild";
const campaginsUrl = "/Api/GetActiveCampaigns";
const searchEmployeeUrl = "/Api/SearchEmployee";

//NOTE filter urls
const omurUrl = "/Api/GetOmurList";
const mantagheUrl = "/Api/GetMantagheByOmur/";
const hozeUrl = "/Api/GetHozeByMantaghe/";
const branchUrl = "/Api/GetBranchByHoze/";

const getChildEmployeesUrl = "/Api/GetChildEmployees";
const getChildAssigneeUrl = "/Api/GetChildAssignee";
const postAssignment = "/Api/Assignment";
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
