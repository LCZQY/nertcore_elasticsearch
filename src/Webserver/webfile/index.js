jQuery.Public_Url = "https://localhost:3001/";

/**
 * @param {string} url -- 请求路径
 * @param {Object} data -- 请求参数
 * @param {Function} successCallBack -- 成功回调
 * @param {Function} errorCallBack -- 失败回调
 * @param {boolean} bool -- 是否异步调用
 */
var PostData = function(url, data, successCallBack, errorCallBack, bool) {
	/*true异步刷新ajax、false同步刷新ajax*/
	var boo = true;
	/*判断bool是否为布尔类型*/
	if(bool) {
		if(typeof bool == "boolean") {
			boo = bool;
		}
	}
	errorCallBack = errorCallBack || function() {
		
	};
	$.ajax({
		type: 'POST',
		url: url,
		data: data,
		async: boo,
		dataType: "json",
		cache: false,
		crossDomain: true,
		contentType: "application/json;charset=UTF-8",
		success: function(jsonData) {
			/*返回数据类型处理为对象格式*/
			if(typeof jsonData === 'string') {
				try {
                    successCallBack(JSON.parse(jsonData));
				}catch(e) {
 
				}
			} else {
				successCallBack(jsonData);
			}
		},
		error: errorCallBack
    });
}
    
/**
 * @param {string} url -- 请求路径
 * @param {Object} data -- 请求参数
 * @param {Function} successCallBack -- 成功回调
 * @param {Function} errorCallBack -- 失败回调
 * @param {boolean} bool -- 是否异步调用
 */
var GetData = function(url, data, successCallBack, errorCallBack, bool) {
	/*true异步刷新ajax、false同步刷新ajax*/
	var boo = true;
	/*判断bool是否为布尔类型*/
	if(bool) {
		if(typeof bool == "boolean") {
			boo = bool;
		}
	}
	errorCallBack = errorCallBack || function() {
		
	};
	$.ajax({
		type: 'GET',
		url: url,
		data: data,
		async: boo,
		dataType: "json",
		cache: false,
		crossDomain: true,
		contentType: "application/x-www-form-urlencoded;charset=UTF-8",
		success: function(jsonData) {
			/*返回数据类型处理为对象格式*/
			if(typeof jsonData == 'string') {
				try {
					successCallBack(JSON.parse(jsonData));
				} catch(e) {
 
				}
			} else {
				successCallBack(jsonData);
			}
		},
		error: errorCallBack
    });
}

/**
 * 构建分页html
 * */
var pagehtml = function () {

    var index = 0;
    $("#nextpage").click(function () {
        index += 1;

        PostData($.Public_Url + "api/values/list", JSON.stringify({
            PageIndex: index,
            PageSize: 10
        }), function (jsonData) {
            if (jsonData.code == "0") {
                var html = "";
                $.each(jsonData.extension, function (index, value) {
                    html += listhtml(value);
                });
                $("#sellListContent").html(html)
                $("#totalCount").text(jsonData.totalCount);
                $("#index").text(index)
            }
        });
    });
};


/**
 * 构建房源列表html
 */
var listhtml = function (options) {
    console.log(options,"本次的");
    var html = "";
    html += '<li class="clear">';
    html += '<a class="img  CLICKDATA maidian-detail" data-agentid="' + options.id + '">';
    html += '<img class="lj-lazy" src="' + options.icon + '" alt="' + options.summary + '" title="options.Summary" style="display: block;"></a>';
    html += '<div class="info clear"><div class="title">';
    html += '<a class="VIEWDATA CLICKDATA maidian-detail" title="options.Summary" data-agentid="options.Id" href="javascript:;" target="_blank">' + options.name + '</a> </div>';
    html += '<div class="address">';
    html += '<div class="flood">';
    html += '<div class="positionInfo">';
    html += '<span class="positionIcon"></span>';
    html += ' <a href="https://cq.ke.com/xiaoqu/3611060491220/">' + options.address + '</a>';
    html += '</div></div>';
    html += '<div class="houseInfo"><span class="houseIcon"></span>' + options.builtupArea + ' 平米</div>';
    html += '<div class="followInfo"><span class="starIcon"></span>' + options.createTime + '</div>';
    html += '<div class="tag"><span class="subway">近地铁</span></div>';

    html += ' <div class="priceInfo">';
    html += '<div class="totalPrice"><span>' + options.maxPrice + '</span>万</div>';
    html += ' <div class="unitPrice" data-hid="19051018110100223583" data-price="13268.6">';
    html += '<span>单价</span>' + options.minPrice + '<span>/平方米</span></div></div></div>';
    html += '<div class="listButtonContainer">';
    html += '<div class="btn-follow followBtn CLICKDATA" data-hid="19051018110100223583" data-click-evtid="11284" data-click-event="FavoriteClick" >';
    html += '<span class="follow-text">关注</span> </div></div></div></li>';
    return html;
}


/**
 *  楼盘列表渲染页面
 * @param {any} jsonData List<buildingInfo>
 */
var CreateBuildingList = function (jsonData) {


    var html = "";
    $.each(jsonData.extension, function (index, value) {
        html += listhtml(value);
    });
    $("#sellListContent").html(html);
    $("#totalCount").text(jsonData.totalCount);
    $("#pageindex").text(jsonData.PageIndex);
};