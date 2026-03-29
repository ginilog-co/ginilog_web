"use client";
"use strict";
exports.__esModule = true;
exports.Navbar = void 0;
var link_1 = require("next/link");
var react_1 = require("react");
var lucide_react_1 = require("lucide-react");
var navLinks = [
    { href: "/landing-page", label: "Home" },
    { href: "/landing-page#contact", label: "Contact Us" },
];
function Navbar() {
    var _a = react_1.useState(false), isOpen = _a[0], setIsOpen = _a[1];
    return (React.createElement("nav", { className: "bg-white border-b sticky top-0 z-50" },
        React.createElement("div", { className: "container mx-auto px-4" },
            React.createElement("div", { className: "flex items-center justify-between h-16" },
                React.createElement(link_1["default"], { href: "/landing-page", className: "text-xl font-bold text-primary" }, "GINILOG"),
                React.createElement("div", { className: "hidden md:flex items-center space-x-8" },
                    navLinks.map(function (link) { return (React.createElement(link_1["default"], { key: link.href, href: link.href, className: "text-gray-600 hover:text-primary transition-colors" }, link.label)); }),
                    React.createElement(link_1["default"], { href: "/customer-portal", className: "bg-primary text-white px-4 py-2 rounded hover:bg-primary/90 transition-colors" }, "Customer Portal"),
                    React.createElement("a", { href: "https://manager-web.ginilog.com/", target: "_blank", rel: "noopener noreferrer", className: "bg-secondary text-white px-4 py-2 rounded hover:bg-secondary/90 transition-colors" }, "Manage Services")),
                React.createElement("button", { onClick: function () { return setIsOpen(!isOpen); }, className: "md:hidden p-2", "aria-label": "Toggle menu" }, isOpen ? React.createElement(lucide_react_1.X, { size: 24 }) : React.createElement(lucide_react_1.Menu, { size: 24 }))),
            isOpen && (React.createElement("div", { className: "md:hidden py-4 border-t" },
                React.createElement("div", { className: "flex flex-col space-y-4" },
                    navLinks.map(function (link) { return (React.createElement(link_1["default"], { key: link.href, href: link.href, className: "text-gray-600 hover:text-primary transition-colors", onClick: function () { return setIsOpen(false); } }, link.label)); }),
                    React.createElement(link_1["default"], { href: "/customer-portal", className: "bg-primary text-white px-4 py-2 rounded hover:bg-primary/90 transition-colors text-center" }, "Customer Portal"),
                    React.createElement("a", { href: "https://manager-web.ginilog.com/", target: "_blank", rel: "noopener noreferrer", className: "bg-secondary text-white px-4 py-2 rounded hover:bg-secondary/90 transition-colors text-center" }, "Manage Services")))))));
}
exports.Navbar = Navbar;
