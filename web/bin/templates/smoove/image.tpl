<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
	<head>
		<title>@@PAGE_TITLE@@</title>
		<link rel="stylesheet" href="@@PAGE_CSS@@" type="text/css"/>
	</head>
	
	<body>
			<div id="header">
				<div id="headerTop">
					<div>
					        <span id="title">@@PAGE_TITLE@@</span>
						<br/>
						<span id="url"><a href="@@PAGE_URL@@">@@PAGE_URL@@</a></span>
					</div>
				</div>
				<div id="headerBottom">
		 		   	<div id="parentLink">
						<a href="@@PARENT_URL@@">back to directory listing</a>
					</div>
				</div>
			</div>
	
			<ul id="sizeList">
				<!-- @@SIZE_ITEM@@ -->
				<li><a href="@@SIZE_URL@@">@@SIZE_TITLE@@</a></li>
				<!-- @@SIZE_ITEM@@ -->
			</ul>
	
			<div id="errors">
				<!-- @@ERROR@@ -->
				<p>@@ERROR_TEXT@@</p>
				<!-- @@ERROR@@ -->
			</div>
	
		        <div id="image">
				<a href="@@PIC_URL@@">
					<img src="@@PIC_SRC@@" border="0" alt="@@PIC_ALT@@"/>
				</a>
				<p>@@PIC_DESCRIPTION@@</p>

				<table id="exifInfo">
					<tr>
						<th colspan="2">Image Information</th>
					</tr>
					<!-- @@EXIF_ITEM@@ -->
					<tr>
						<td>@@EXIF_NAME@@:</td>
						<td>@@EXIF_VALUE@@</td>
					</tr>
					<!-- @@EXIF_ITEM@@ -->
				</table>

			</div>
	
			<div id="footer">
				Powered by <a href="@@QUICKTHUMBS_URL@@">@@QUICKTHUMBS_VERSIONSTRING@@</a>.
			</div>
	</body>
</html>
