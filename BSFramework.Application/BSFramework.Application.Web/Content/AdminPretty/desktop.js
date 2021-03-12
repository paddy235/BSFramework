function bindChart(rows) {
    var array = eval('(' + rows + ')');
    var name = new Array();
    var count = new Array();
    for (var i in array) {
        name.push(array[i].category);
        count.push(array[i].value);
    }
    var n = echarts.init(document.getElementById("mainChart")), t;
    //var ecConfig = echarts.config;
    //var ecConfig1 = require('echarts/config');
    //function eConsole(param) {
    //    if (typeof param.seriesIndex != 'undefined') {
    //        alert(param.name);
    //    }
    //}
    //n.on(ecConfig.EVENT.CLICK, eConsole);
    n.setOption(
        {
            backgroundColor: "#fff",
            title: {
                text: "月度违章次数统计",
                left: 15,
                top: 15,
                textStyle: {
                    color: "#666666",
                    fontSize: 24,
                    fontWeight: 100,
                    fontFamily: "微软雅黑"
                },
                subtext: "",
                subtextStyle: {
                    color: "#878787", fontSize: 14, fontWeight: 100, fontFamily: "微软雅黑"
                },
                itemGap: 20
            },
            tooltip: {
                trigger: "axis",
                axisPointer: {
                    type: "shadow",
                    textStyle: { color: "#fff" }
                }
            },
            //tooltip:
            //    {
            //        formatter: function (a) {
            //            return (a['seriesName'] + '</br>' + a['name'] + ':' + a['value']);
            //        }
            //    },
            legend: {
                //data: ["违章次数"],
                //x: 15, y: 45, textStyle: { color: "#878787" }
            },
            grid: {
                borderWidth: 0, top: 100, left: "2%", right: "1%", bottom: "12%", containLabel: !0
            },
            xAxis: [{
                type: "category",
                data: name,
                splitLine: !1, axisTick: !1,
                axisLabel: {
                    interval: 0,
                    rotate: 40,
                    textStyle: {}
                }
            }],
            yAxis: [{
                type: "value",
                splitLine: !1, axisTick: !1,
                splitNumber: 1
            }],
            series: [{
                name: "违章次数", type: "bar", label: { normal: { show: true, position: "top", textStyle: { color: "#87A1F6" } } },
                itemStyle: { normal: { color: "#DDE0E5" } },
                data: count
            }]
        });
    
    //myChart1 = echarts.init(document.getElementById("echart1"));
    //t = {
    //    backgroundColor: "#fff",
    //    title: {
    //        text: "班会任务完成率", left: 15, top: 15,
    //        textStyle: {
    //            color: "#666666", fontSize: 24, fontWeight: 100, fontFamily: "微软雅黑"
    //        }
    //    },
    //    tooltip: {
    //        trigger: "axis", axisPointer: { type: "shadow" }
    //    },
    //    legend: {
    //        data: ["次数"], x: "right", y: 30, padding: [5, 35, 5, 5]
    //    },
    //    grid: {
    //        borderWidth: 0, top: 100, left: "3%", right: "4%", bottom: "3%", containLabel: !0
    //    },
    //    xAxis: [{
    //        type: "category", data: ["周一", "周二", "周三", "周四", "周五", "周六", "周日"], splitLine: !1, axisLine: !1, axisTick: !1
    //    }],
    //    yAxis: [{
    //        type: "value", splitLine: !1, axisLine: !1, axisTick: !1
    //    }],
    //    series: [{
    //        name: "其它", type: "bar", itemStyle: { normal: { color: "#DDE0E5" } }, data: [400, 500, 600, 700, 500, 400, 310]
    //    }]
    //};
    //myChart1.setOption(t);
    //myChart2 = echarts.init(document.getElementById("echart2"));
    //option2 = {
    //    backgroundColor: "#fff", title: {
    //        text: "客户区域分布", left: 15, top: 15,
    //        textStyle: {
    //            color: "#666666", fontSize: 24, fontWeight: 100, fontFamily: "微软雅黑"
    //        }
    //    },
    //    tooltip: {
    //        trigger: "item", formatter: "{a} <br/>{b}: {c} ({d}%)"
    //    },
    //    toolbox: {
    //        show: !0, feature: { restore: { show: !0, title: "还原" }, saveAsImage: { show: !0, title: "保存为图片", type: "png", lang: ["点击保存"] } }, padding: [25, 25, 5, 5]
    //    },
    //    legend: {
    //        x: "left", y: "bottom", data: ["上海", "苏州", "山东", "其它"], padding: [5, 5, 35, 20]
    //    },
    //    series: [{
    //        name: "访问来源", type: "pie", radius: ["45%", "65%"],
    //        avoidLabelOverlap: !1, selectedMode: "single",
    //        label: {
    //            normal: { show: !1, position: "center" }, emphasis: { show: !0, textStyle: { fontSize: "30", fontWeight: "bold" } }
    //        },
    //        labelLine: { normal: { show: !1 } },
    //        data: [
    //            {
    //                value: 335, name: "其它", itemStyle: { normal: { color: "#2AE8DE" } }
    //            }, {
    //                value: 234, name: "山东", itemStyle: { normal: { color: "#f06995" } }
    //            }, {
    //                value: 135, name: "苏州", itemStyle: { normal: { color: "#F2B459" } }
    //            }, {
    //                value: 1548, name: "上海", itemStyle: { normal: { color: "#7b93e0" } }
    //            }]
    //    }]
    //};
    //myChart2.setOption(option2); window.onresize = function (t) { n.resize(t); myChart1.resize(t); myChart2.resize(t) }
}