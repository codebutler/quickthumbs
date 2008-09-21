<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
	<head>
		<title>@@PAGE_TITLE@@</title>
		<link rel="stylesheet" href="@@PAGE_CSS@@" type="text/css"/>
	</head>
	
	<body>
		<div id="content">
			<div id="header">
				<div id="headerTop">
				        <div id="title">
					        @@PAGE_TITLE@@
					</div>
					<div id="imageCount">
						@@PAGE_DIRCOUNT@@ directories, @@PAGE_IMAGECOUNT@@ images.
					</div>
				</div>
				<div id="headerBottom">
		 		   	<div id="parentLink">
						<a href="..">up to parent directory</a>
					</div>
				</div>
			</div>
	
			<div id="dirBox">
				
				<!-- @@DIR_ITEM@@ -->
				<div class="listItem dirItem">
					<a href="@@DIR_URL@@">@@DIR_NAME@@</a>
				</div>
				<!-- @@DIR_ITEM@@ -->

			</div>
	
			<!-- @@LISTS_SEPORATOR@@ -->
			<hr/>
			<!-- @@LISTS_SEPORATOR@@ -->


	   		<div id="picBox">
			
				<!-- @@PIC_ITEM@@ -->
			        <div class="listItem picItem">
					<a href="@@PIC_URL@@">
						<img src="@@PIC_THUMB_URL@@" width="@@PIC_THUMB_WIDTH@@" height="@@PIC_THUMB_HEIGHT@@" alt="@@PIC_NAME@@"/>
					</a>
					<div>
						<span class="picName">@@PIC_NAME@@</span>
						<br/>
						<span class="picSize">size: @@PIC_SIZE@@</span>
	        			</div>
				</div>
				<!-- @@PIC_ITEM@@ -->
				
			</div>
	
			<div id="footer">
				Powered by <a href="@@QUICKTHUMBS_URL@@">@@QUICKTHUMBS_VERSIONSTRING@@</a>.
			</div>
		</div>
	</body>
</html>
