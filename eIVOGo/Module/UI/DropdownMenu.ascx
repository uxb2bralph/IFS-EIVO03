﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="Uxnet.Web.Module.ForBootstrap.DropdownMenu" %>
<%@ Register Src="DropdownMainMenuBlock.ascx" TagName="DropdownMenuBlock" TagPrefix="uc1" %>
<style type="text/css">
    .nav-side-menu {
        overflow: auto;
        font-family: verdana;
        font-size: 12px;
        font-weight: 200;
        background-color: #E7E8D5;
        position: fixed;
        top: 0px;
        width: 215px;
        height: 100%;
        color: #5c6978;
    }

        .nav-side-menu .brand {
            background-color: #FEF3AF;
            line-height: 50px;
            display: block;
            text-align: center;
            font-size: 14px;
            color: #8a4000;
        }

        .nav-side-menu .toggle-btn {
            display: none;
        }

        .nav-side-menu ul,
        .nav-side-menu li {
            list-style: none;
            padding: 0px;
            margin: 0px;
            line-height: 35px;
            cursor: pointer;
            /*    
    .collapsed{
       .arrow:before{
                 font-family: FontAwesome;
                 content: "\f053";
                 display: inline-block;
                 padding-left:10px;
                 padding-right: 10px;
                 vertical-align: middle;
                 float:right;
            }
     }
*/
        }

            .nav-side-menu ul :not(collapsed) .arrow:before,
            .nav-side-menu li :not(collapsed) .arrow:before {
                font-family: FontAwesome;
                content: "\f078";
                display: inline-block;
                padding-left: 10px;
                padding-right: 10px;
                vertical-align: middle;
                float: right;
            }

            .nav-side-menu ul .active,
            .nav-side-menu li .active {
                border-left: 3px solid #d19b3d;
                background-color: #DCE5C6;
            }

            .nav-side-menu ul .sub-menu li.active,
            .nav-side-menu li .sub-menu li.active {
                color: #d19b3d;
            }

                .nav-side-menu ul .sub-menu li.active a,
                .nav-side-menu li .sub-menu li.active a {
                    color: #d19b3d;
                }

            .nav-side-menu ul .sub-menu li,
            .nav-side-menu li .sub-menu li {
                background-color: #FEF3AF;
                border: none;
                line-height: 28px;
                border-bottom: 1px solid #23282e;
                margin-left: 0px;
            }

                .nav-side-menu ul .sub-menu li:hover,
                .nav-side-menu li .sub-menu li:hover {
                    background-color: #B2D46F;
                }

                .nav-side-menu ul .sub-menu li:before,
                .nav-side-menu li .sub-menu li:before {
                    font-family: FontAwesome;
                    content: "\f105";
                    display: inline-block;
                    padding-left: 10px;
                    padding-right: 10px;
                    vertical-align: middle;
                }

        .nav-side-menu li {
            padding-left: 0px;
            border-left: 3px solid #2e353d;
            border-bottom: 1px solid #23282e;
        }

            .nav-side-menu li a {
                text-decoration: none;
                color: #5c6978;
            }

                .nav-side-menu li a i {
                    padding-left: 10px;
                    width: 20px;
                    padding-right: 20px;
                }

            .nav-side-menu li:hover {
                border-left: 3px solid #d19b3d;
                background-color: #B2D46F;
                -webkit-transition: all 1s ease;
                -moz-transition: all 1s ease;
                -o-transition: all 1s ease;
                -ms-transition: all 1s ease;
                transition: all 1s ease;
            }

    @media (max-width: 767px) {
        .nav-side-menu {
            position: relative;
            width: 100%;
            margin-bottom: 10px;
        }

            .nav-side-menu .toggle-btn {
                display: block;
                cursor: pointer;
                position: absolute;
                right: 10px;
                top: 10px;
                z-index: 10 !important;
                padding: 3px;
                background-color: #ffffff;
                color: #000;
                width: 40px;
                text-align: center;
            }

        .brand {
            text-align: left !important;
            font-size: 22px;
            padding-left: 20px;
            line-height: 50px !important;
        }
    }

    @media (min-width: 767px) {
        .nav-side-menu .menu-list .menu-content {
            display: block;
        }
    }

    body {
        margin: 0px;
        padding: 0px;
    }
</style>
<div class="nav-side-menu">
    <div class="brand"><%= MenuBrand %></div>
    <i class="fa fa-bars fa-2x toggle-btn" data-toggle="collapse" data-target="#menu-content"></i>
    <div class="menu-list">
        <uc1:DropdownMenuBlock ID="menuBlock" runat="server" />
    </div>
</div>
<script runat="server">
    [System.ComponentModel.Bindable(true)]
    public String MenuBrand
    { get; set; }
</script>
