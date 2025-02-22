//SECTION - base data elements
const campaigns = document.getElementById("campaignFilter");
const omur = document.getElementById("omurFilter");
const mantaghe = document.getElementById("mantagheFilter");
const hoze = document.getElementById("hozeFilter");
const branch = document.getElementById("branchFilter");
const plugin = document.getElementById("pluginFilter");
const assignedBy = document.getElementById("assignedByFilter");
const customerNumber = document.getElementById("customerNumberFilter");
//!SECTION - base data elements

const currentCampaignId = document.getElementById("currentCampaignId");

//SECTION - data url
const campaginsUrl = "/Api/GetCampaigns";
const omurUrl = "/Api/GetOmurList";
const mantagheUrl = "/Api/GetMantagheByOmur/";
const hozeUrl = "/Api/GetHozeByMantaghe/";
const branchUrl = "/Api/GetBranchByHoze/";
const getHozeByBranchUrl = "/Api/GetHozeByBranch/";
const getPluginsUrl = "/Api/GetPlugins/";
//!SECTION - data url
