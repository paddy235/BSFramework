/// <summary>
/// 选择部门 
/// </summary>
///<param name="deptId">查询条件，根据mode的值查询方式会不同</param>
/// <param name="checkMode">单选或多选，0:单选，1:多选</param>
/// <param name="mode">查询模式，0:获取机构ID为Ids下所有的部门(即EnCode=Ids)，1:获取部门ParentId为Ids下所有部门(即ParentId=Ids)，2:获取部门的parentId在Ids中的部门(即EnCode in(Ids))，3:获取部门Id在Ids中的部门(即DepartmentId in(Ids))4.获取承包商和分包商</param>
///<param name="title">弹出层标题</param>
///<param name="winObject">窗体中需要查找domId的对象，一般可写成window.document.body或this.parentNode</param>
///<param name="domId">接收返回值的dom节点的ID，多个用逗号分隔，顺序依次为部门名称,部门Code，部门Id,部门负责人姓名,部门负责人ID（多选用逗号分隔）</param>
function selectDept(url, deptId, checkMode, mode, title, winObject, domId, category, cb) {
    var args = jQuery.param({ deptId: deptId, checkMode: checkMode, mode: mode, category: category });
    return dialogOpen({
        id: "Dept",
        title: title,
        url: url + (url.indexOf('?') > 0 ? '&' : '?') + args,
        width: "700px",
        height: "500px",
        callBack: function (iframeId) {
            if (cb) cb();
            top.document.getElementById(iframeId).contentWindow.AcceptClick(winObject, domId, window);
        }
    });
}

function selectDeptByArgs(deptId, checkMode, mode, title, winObject, domId, action) {
    return dialogOpen({
        id: "Dept",
        title: title,
        url: '/BaseManage/Department/Select?deptId=' + deptId + "&checkMode=" + checkMode + "&mode=" + mode,
        width: "700px",
        height: "500px",
        callBack: function (iframeId) {
            top.document.getElementById(iframeId).contentWindow.PostSubmit(winObject, domId);
        }
    });
}

function showModa(url, title, width, height, arg, callback) {
    var id = 'iframe' + new Date().getTime();
    return dialogOpen({
        id: id,
        title: title,
        url: url + '?' + jQuery.param(arg),
        width: width,
        height: height,
        callBack: function () {
            top.document.getElementById(id).contentWindow.fn$ok(callback);
        }
    });
}
/// <summary>
/// 选择用户
/// </summary>
///<param name="deptId">查询条件，根据mode的值查询方式会不同</param>
/// <param name="checkMode">单选或多选，0:单选，1:多选</param>
/// <param name="mode">查询模式，0:获取机构ID为Ids下所有的部门(即OrganizeId=Ids)，1:获取部门ParentId为Ids下所有部门(即ParentId=Ids)，2:获取部门的parentId在Ids中的部门(即ParentId in(Ids))，3:获取部门Id在Ids中的部门(即DepartmentId in(Ids))</param>
///<param name="winObject">窗体中需要查找domId的对象，一般可写成window.document.body或this.parentNode</param>
///<param name="domId">接收返回值的dom节点的ID，多个用逗号分隔，顺序依次为用户名称,用户账号，用户Id（多选用逗号分隔）</param>
function selectUser(options) {
    var deptCode = options.deptCode == undefined ? "" : options.deptCode;
    return dialogOpen({
        id: "User",
        title: "选择用户",
        url: '/BaseManage/User/Select?deptId=' + options.deptId + "&checkMode=" + options.checkMode + "&mode=" + options.mode + "&deptCode=" + deptCode,
        width: ($(top.window).width() - 200) + "px",
        height: "600px",
        callBack: function (iframeId) {
            top.document.getElementById(iframeId).contentWindow.AcceptClick(options);
        }
    });
}

//选择不同用户
function selectDifferentUser(deptId, checkMode, mode, winObject, domId, dfferentID, describe) {
    return dialogOpen({
        id: "User",
        title: "选择用户",
        url: '/BaseManage/User/Select?deptId=' + deptId + "&checkMode=" + checkMode + "&mode=" + mode,
        width: ($(top.window).width() - 200) + "px",
        height: "600px",
        callBack: function (iframeId) {
            top.document.getElementById(iframeId).contentWindow.AcceptDifferentClick(winObject, domId, dfferentID, describe);
        }
    });
}

///options.depts 部门
///options.checkMode single/multiple
///options.height 高度
///options.mode 0/1 本部门及下所有部门
function selectUsers(options) {
    return dialogOpen({
        id: "User",
        title: "选择用户",
        url: options.url + '?deptId=' + options.depts + "&checkMode=" + (options.checkMode == 'multiple' ? '1' : '0') + '&mode=' + options.mode,
        width: ($(top.window).width() - 200) + "px",
        height: options.height ? options.height + 'px' : "600px",
        callBack: function (iframeId) {
            top.document.getElementById(iframeId).contentWindow.AcceptClick(options);
        }
    });
}

/// <summary>
/// 选择角色
/// </summary>
///<param name="deptId">查询条件，根据mode的值查询方式会不同</param>
/// <param name="checkMode">单选或多选，0:单选，1:多选</param>
/// <param name="mode">查询模式，0:获取机构ID为Ids下所有的部门(即OrganizeId=Ids)，1:获取部门ParentId为Ids下所有部门(即ParentId=Ids)，2:获取部门的parentId在Ids中的部门(即ParentId in(Ids))，3:获取部门Id在Ids中的部门(即DepartmentId in(Ids))</param>
///<param name="winObject">窗体中需要查找domId的对象，一般可写成window.document.body或this.parentNode</param>
///<param name="domId">接收返回值的dom节点的ID，多个用逗号分隔，顺序依次为用户名称,用户账号，用户Id（多选用逗号分隔）</param>
function selectRole(roleIDs, deptId, checkMode, mode, winObject, domId) {
    return dialogOpen({
        id: "Role",
        title: "选择角色",
        url: '/BaseManage/Role/Select?roleIDs=' + roleIDs + '&deptId=' + deptId + "&checkMode=" + checkMode + "&mode=" + mode,
        width: "250px",
        height: "500px",
        callBack: function (iframeId) {
            top.document.getElementById(iframeId).contentWindow.AcceptClick(winObject, domId);
        }
    });
}
/// <summary>
/// 下载导出文件
/// </summary>
function downloadFile(downurl) {
    var $exp = $("#expFrame");
    if ($exp.length == 0) {
        $exp = $("<iFrame id='expFrame' style='display:none;'/>");
        $("body").append($exp);
    }
    $exp.attr({ src: downurl });
}



