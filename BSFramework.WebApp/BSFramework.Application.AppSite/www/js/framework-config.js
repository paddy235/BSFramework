/**
 * Created by cbb on 16/6/3.
 */
//接口地址信息
angular.module('starter.config', [])
.factory('ApiUrl', function () {
    var rootUrl = "http://localhost:62831/bsframework/api";
  var apiUrl = {
      loginApi: rootUrl + "/login/checkLogin",
      //退出
      outLoginApi: rootUrl + '/login/outLogin',
      //获取员工列表
      getUserListApi:rootUrl +'/user/getUserList',
      //商机列表
      chanceListApi: rootUrl + '/customerManage/chanceList',
      //客户列表
      customerListApi: rootUrl + '/customerManage/customerList',
      //数据字典列表接口
      dataItemListApi: rootUrl + '/dataItem/list',
      //商机添加
      saveChanceApi: rootUrl + '/customerManage/saveChance',
      //客户添加
      saveCustomerApi: rootUrl + '/customerManage/saveCustomer',
      //通知公告列表
      noticeListApi: rootUrl + '/publicInfoManage/noticeList',
      //区域列表接口
      areaListApi: rootUrl + '/area/list',
      //商机删除
      deleteChanceApi: rootUrl + '/customerManage/deleteChance',
      //客户删除
      deleteCustomerApi: rootUrl + '/customerManage/deleteCustomer',



      //发起流程列表接口
      flowDesignListApi: rootUrl + '/flowDesign/list',
      //发起流程添加接口
      saveFlowDesignApi: rootUrl + '/flowDesign/list',
      //草稿流程列表接口
      flowRoughdraftListApi: rootUrl + '/flowRoughdraft/list',
      //草稿流程删除接口
      deleteFlowRoughdraftApi: rootUrl + '/flowRoughdraft/list',
      //我的流程列表接口
      flowProcessListApi: rootUrl + '/flowProcess/list',
      //待办流程列表接口
      flowBefProcessListApi: rootUrl + '/flowBefProcess/list',
      //代办流程审核接口
      saveFlowBefProcessApi: rootUrl + '/flowBefProcess/list',
      //已办流程列表接口
      flowAftProcessListApi: rootUrl + '/flowAftProcess/list',
      //工作委托列表接口
      flowDelegateListApi: rootUrl + '/flowDelegate/list',
      //订单列表
      getOrderListApi: rootUrl + '/customerManage/list',
      //收款管理
      getReceivableListApi: rootUrl + '/customerManage/list',
      //收款报表管理
      getReceivableReportListApi: rootUrl + '/customerManage/list'
  };
  return apiUrl;
})
.factory('SignalrUrl',function(){
    return "http://localhost:8081/signalr";
    //return "http://localhost:8081/signalr";
})
;
