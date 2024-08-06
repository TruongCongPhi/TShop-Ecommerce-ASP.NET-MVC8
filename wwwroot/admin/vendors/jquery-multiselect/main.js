$(function() {

	$('select[multiple].active.3col').multiselect({
	  columns: 3,
	  placeholder: 'Chọn màu',
	  search: true,
	  searchOptions: {
	      'default': 'Tìm kiếm'
	  },
	  selectAll: true
	});

});