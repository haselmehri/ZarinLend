//SECTION - steps of wizard
let campaignStep = document.getElementById("campaignStep");
let accStep = document.getElementById("accStep");
let customerStep = document.getElementById("customerStep");

let cStep = document.getElementById("cStep");
let aStep = document.getElementById("aStep");
let cuStep = document.getElementById("cuStep");
//!SECTION - steps of wizard

//NOTE - alert element
let alertElement = document.getElementById("alert");

//SECTION - base data elements
let campaigns = document.getElementById("campaigns");
let accuntManagers = document.getElementById("accuntManagers");
//!SECTION - base data elements

//SECTION - employee selection
let employee = document.getElementById("employee");
let employeeInfo = document.getElementById("employeeInfo");
let selectedEmployee = document.getElementById("selectedEmployee");
let employeeData = document.getElementById("employeeData");
let angleEmp = document.getElementById("angleEmp");
let selectedE = document.getElementById("selectedE");
let anlgeEButton = document.getElementById("anlgeEButton");
//!SECTION - employee selection

//SECTION - current employee data
let currentEmployeeId = document.getElementById("currentEmployeeId").value;
let currentEmployeeRole = document.getElementById("role").value;
let currentEmployeeDepartment = document.getElementById("department").value;
let currentEmployeeRegion = document.getElementById("region").value;
let currentEmployeeArea = document.getElementById("area").value;
let currentEmployeeBranch = document.getElementById("branch").value;
//!SECTION - current employee data

//SECTION - employee filters
let omurFilter = document.getElementById("omurFilter");
let mantagheFilter = document.getElementById("mantagheFilter");
let hozeFilter = document.getElementById("hozeFilter");
let branchFilter = document.getElementById("branchFilter");
let btnAccFilter = document.getElementById("btnAccFilter");
//!SECTION - employee filters

//SECTION - pagination
let employeeTotal = document.getElementById("employeeTotal");
let employeeCurrent = document.getElementById("employeeCurrent");

let employeePagingTop = document.getElementById("employeePagingTop");
let employeePagingBottom = document.getElementById("employeePagingBottom");

let totalPagesT = document.getElementById("totalPagesT");
let currentPageT = document.getElementById("currentPageT");
let totalPagesB = document.getElementById("totalPagesB");
let currentPageB = document.getElementById("currentPageB");

let firstT = document.getElementById("firstT");
let perviewsT = document.getElementById("perviewsT");
let nextT = document.getElementById("nextT");
let lastT = document.getElementById("lastT");

let firstB = document.getElementById("firstB");
let perviewsB = document.getElementById("perviewsB");
let nextB = document.getElementById("nextB");
let lastB = document.getElementById("lastB");
//!SECTION - pagination

//SECTION - employee data tabel
let personels = document.getElementById("personels");
//!SECTION - employee data tabel

//SECTION - data url
let accManagerUrl = "/Api/GetRoleChild";
let campaginsUrl = "/Api/GetAllCampaigns";
let searchEmployeeUrl = "/Api/SearchEmployee";

//NOTE filter urls
var omurUrl = "/Api/GetOmurList";
var mantagheUrl = "/Api/GetMantagheByOmur/";
var hozeUrl = "/Api/GetHozeByMantaghe/";
var branchUrl = "/Api/GetBranchByHoze/";

var getChildEmployeesUrl = "/Api/GetChildEmployees";
//!SECTION - data url

let pageNumber = 1;
var dataRequestEmployee = {};
let slide = false;
let setError = false;
const cerror = "کمپین انتخاب نشده!";
const aError = "مدیر حساب انتخاب نشده!";
