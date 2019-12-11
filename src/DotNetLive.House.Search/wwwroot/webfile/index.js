
/**
 * 获取分页html
 * @@param {any} total 总数
 * @@param {any} pageindex  分页数默认10行
 * */
var pagehtml = function (total, pageindex = 10) {

    var size = total / pageindex;
    var html = "";
    for (var i = 1; i < 3; i++) {

        if (i === 1) { html += '<a class="on" href="/ershoufang/c3611060491220" data-page="' + i + '">' + i + '</a>'; } else {
            html += '<a  href="/ershoufang/c3611060491220" data-page="' + i + '">' + i + '</a>';
        }        
    }
    if (size > 3) {
        html += '<span>...</span>' + '<a  href="/ershoufang/c3611060491220" data-page="' + size + '">' + size + '</a>';
    }
    html += '<a href="/ershoufang/pg2rs重庆" data-page="2">下一页</a>';
    return html;
};


