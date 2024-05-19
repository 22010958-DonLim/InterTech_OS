<?php

	$to = "wp.solution.studio@gmail.com"; // this is your Email address
	$from  = $_POST['email']; // this is the sender's Email address
	$sender_name = $_POST['name'];
	$msg = $_POST['message'];

	$subject = "Form submission";
	$message = "Name: " . $sender_name . " - Message: " . $msg;
	
	
	$headers = 'From: ' . $from;
	mail($to, $subject, $message, $headers);

?>