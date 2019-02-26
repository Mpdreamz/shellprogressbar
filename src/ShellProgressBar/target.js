(function ($R) {

	$R.compileTime = '1304957970';

	$R.path = 'http://readable.tastefulwords.com/';
	$R.linksPath = 'http://readable.tastefulwords.com/';

	$R.win = window;

	$R.embedded = ('embeddedOptions' in $R);

	$R.custom = false;
	$R.customOptions = {};

	var	
		_host = $R.win.location.host,
		_path = $R.win.location.pathname,
		_custom = 
		{

				'www.google.com': 			[[/^\/reader\/view\//i, 	'js: google_reader']],
				'www.stumbleupon.com': 		[[/^\/su\//i, 				'js: stumble_upon']],
				'en.wikipedia.org': 		[[/^\/wiki\//i, 			'js: wikipedia_en']],

				'www.boston.com': 			[[/^\//i, 					'js: boston_globe']],
				'boston.com': 				[[/^\//i, 					'js: boston_globe']],

				'www.jornada.unam.mx': 		[[/^\//i, 					'jq: #article-text']],

				'':''
		}
	;

	if (_host in _custom)
	{
		for (var i=0, _i=_custom[_host].length; i<_i; i++)
		{
			if (_path.match(_custom[_host][i][0]) != null)
			{
				var 
					_type = _custom[_host][i][1].substr(0, 2),
					_value = _custom[_host][i][1].substr(4)
				;

				switch (_type)
				{
					case 'js': $R.customOptions['script'] = $R.path + 'custom__'+_value+'-'+$R.compileTime+'.js'; break;
					case 'jq': $R.customOptions['selector'] = _value; break;
				}

				$R.custom = (($R.customOptions['script'] > '') || ($R.customOptions['selector'] > ''));
				break;
			}
		}
	}

	var 
		_iframeElement = document.createElement('iframe'),
		_iframeHTML = ''
		+	'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN"'
		+		' "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">'
		+	'<html id="html" xmlns="http://www.w3.org/1999/xhtml" class="'+($R.embedded ? 'embedded' : '')+'">'
		+	'<head>'

		+		'<link rel="stylesheet" href="'+$R.path+'bulk-'+$R.compileTime+'.css'+'" type="text/css" />'

		+	'</head>'
		+	'<body id="body">'

		+		'<div id="bodyContent"></div>'

		+		'<scr'+'ipt type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js"></scr'+'ipt>'

		+		($R.embedded && ($R.embeddedOptions['script'] > '') ? 
				'<scr'+'ipt type="text/javascript" src="'+$R.embeddedOptions['script']+'"></scr'+'ipt>' : '')

		+		($R.custom && ($R.customOptions['script'] > '') ? 
				'<scr'+'ipt type="text/javascript" src="'+$R.customOptions['script']+'"></scr'+'ipt>' : '')

		+		'<scr'+'ipt type="text/javascript" src="'+$R.path+'bulk-'+$R.compileTime+'.js'+'"></scr'+'ipt>'

		+	'</body>'
		+	'</html>'
	;

	_iframeElement.setAttribute('id', 'readable_app_iframe');
	_iframeElement.setAttribute('frameBorder', '0');
	_iframeElement.setAttribute('allowTransparency', 'true');
	_iframeElement.setAttribute('scrolling', 'auto');

	var 
		_cssElement = document.createElement('style'),
		_cssText = ''
		+	'#readable_app_iframe, #readable_app_cover { '
		+		'margin: 0; padding: 0; border: none; '
		+		'position: absolute; '
		+		'width: 100%; height: 100%; '
		+		'top: -100%; left: 0; '
		+		'z-index: 999999999 !important; '
		+	'} '
		+	'#readable_app_cover { ' 
		+		'z-index: 999999998 !important; '
		+		'margin: 0; padding: 0; border: 0; '
		+	'} '
	;

	_cssElement.setAttribute('id', 'readableCSS1');
	_cssElement.setAttribute('type', 'text/css');
	if (_cssElement.styleSheet) { _cssElement.styleSheet.cssText = _cssText; }
		else { _cssElement.appendChild(document.createTextNode(_cssText)); }

	var _body = document.getElementsByTagName('body')[0];
		_body.appendChild(_cssElement);
		_body.appendChild(_iframeElement);

	var _iframe = document.getElementById('readable_app_iframe'),
		_doc = (_iframe.contentDocument || _iframe.contentWindow.document);
		_doc.open();
		_doc.write(_iframeHTML);
		_doc.close();

})(window.$readable);