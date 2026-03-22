"use strict";
exports.__esModule = true;
exports.Footer = void 0;
var link_1 = require("next/link");
var lucide_react_1 = require("lucide-react");
var quickLinks = [
    { href: "/landing-page", label: "Home" },
    { href: "/landing-page#contact", label: "Contact" },
];
var legalLinks = [
    { href: "/privacy", label: "Privacy Policy" },
    { href: "/terms", label: "Terms of Service" },
];
function Footer() {
    return (React.createElement("footer", { className: "bg-gray-900 text-white py-12" },
        React.createElement("div", { className: "container mx-auto px-4" },
            React.createElement("div", { className: "grid md:grid-cols-4 gap-8" },
                React.createElement("div", null,
                    React.createElement("h5", { className: "text-xl font-semibold mb-4" }, "About Us"),
                    React.createElement("p", { className: "text-gray-400" }, "Ginilog Nigeria Limited is a digital transformation agency focused on turning manual processes to digital processes for businesses, brands and government agencies.")),
                React.createElement("div", null,
                    React.createElement("h5", { className: "text-xl font-semibold mb-4" }, "Quick Links"),
                    React.createElement("ul", { className: "space-y-2" }, quickLinks.map(function (link) { return (React.createElement("li", { key: link.href },
                        React.createElement(link_1["default"], { href: link.href, className: "text-gray-400 hover:text-white transition-colors" }, link.label))); }))),
                React.createElement("div", null,
                    React.createElement("h5", { className: "text-xl font-semibold mb-4" }, "Legal"),
                    React.createElement("ul", { className: "space-y-2" }, legalLinks.map(function (link) { return (React.createElement("li", { key: link.href },
                        React.createElement(link_1["default"], { href: link.href, className: "text-gray-400 hover:text-white transition-colors" }, link.label))); }))),
                React.createElement("div", null,
                    React.createElement("h5", { className: "text-xl font-semibold mb-4" }, "Follow Us"),
                    React.createElement("div", { className: "flex space-x-4" },
                        React.createElement("a", { href: "https://facebook.com/ginilog", target: "_blank", rel: "noopener noreferrer", className: "text-gray-400 hover:text-white transition-colors", "aria-label": "Facebook" },
                            React.createElement(lucide_react_1.Facebook, { size: 24 })),
                        React.createElement("a", { href: "https://twitter.com/ginilog", target: "_blank", rel: "noopener noreferrer", className: "text-gray-400 hover:text-white transition-colors", "aria-label": "Twitter" },
                            React.createElement(lucide_react_1.Twitter, { size: 24 })),
                        React.createElement("a", { href: "https://instagram.com/ginilog", target: "_blank", rel: "noopener noreferrer", className: "text-gray-400 hover:text-white transition-colors", "aria-label": "Instagram" },
                            React.createElement(lucide_react_1.Instagram, { size: 24 })),
                        React.createElement("a", { href: "https://linkedin.com/company/ginilog", target: "_blank", rel: "noopener noreferrer", className: "text-gray-400 hover:text-white transition-colors", "aria-label": "LinkedIn" },
                            React.createElement(lucide_react_1.Linkedin, { size: 24 }))),
                    React.createElement("p", { className: "mt-4 text-gray-400" },
                        React.createElement(lucide_react_1.Mail, { className: "inline mr-2", size: 16 }),
                        "contact@ginilog.com"))),
            React.createElement("div", { className: "border-t border-gray-800 mt-8 pt-8 text-center" },
                React.createElement("p", { className: "text-gray-400" },
                    "\u00A9 ",
                    new Date().getFullYear(),
                    " GINILOG. All rights reserved.")))));
}
exports.Footer = Footer;
