jQuery.extend({
    getUrlArgs: function (name) {
		var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)');
		var r = document.location.search.substr(1).match(reg);
		if (r) return decodeURIComponent(r[2]);
		return null;
	}
})