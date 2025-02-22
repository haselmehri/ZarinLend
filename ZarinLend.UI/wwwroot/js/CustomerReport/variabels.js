//SECTION - data Filters
const omurFilter = document.getElementById("omurFilter");
const mantagheFilter = document.getElementById("mantagheFilter");
const hozeFilter = document.getElementById("hozeFilter");
const branchFilter = document.getElementById("branchFilter");
const campaignFilter = document.getElementById("campaignFilter");
const connectionFilter = document.getElementById("connectionFilter");

const btnFilter = document.getElementById("btnFilter");
//!SECTION - data Filters

//SECTION - employee filters
const currentCampaignId = document.getElementById("currentCampaignId").value;
const currentEmployeeId = document.getElementById("currentEmployeeId").value;
const currentEmployeeRole = document.getElementById("role").value;
const currentEmployeeDepartment = document.getElementById("department").value;
const currentEmployeeRegion = document.getElementById("region").value;
const currentEmployeeArea = document.getElementById("area").value;
const currentEmployeeBranch = document.getElementById("branch").value;
//!SECTION - employee filters

//SECTION - data url
const campaginsUrl = "/Api/GetActiveCampaigns";

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
//!SECTION - data url
