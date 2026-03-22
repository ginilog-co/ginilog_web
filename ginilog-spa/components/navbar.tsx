"use client"

import Link from "next/link";
import { useState } from "react";
import { Menu, X } from "lucide-react";
import { cn } from "@/lib/utils";

const navLinks = [
  { href: "/landing-page", label: "Home" },
  { href: "/landing-page#contact", label: "Contact Us" },
];

export function Navbar() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <nav className="bg-white border-b sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link href="/landing-page" className="text-xl font-bold text-primary">
            GINILOG
          </Link>

          <div className="hidden md:flex items-center space-x-8">
            {navLinks.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className="text-gray-600 hover:text-primary transition-colors"
              >
                {link.label}
              </Link>
            ))}
            <Link
              href="/customer-portal"
              className="bg-primary text-white px-4 py-2 rounded hover:bg-primary/90 transition-colors"
            >
              Customer Portal
            </Link>
            <a
              href="https://manager-web.ginilog.com/"
              target="_blank"
              rel="noopener noreferrer"
              className="bg-secondary text-white px-4 py-2 rounded hover:bg-secondary/90 transition-colors"
            >
              Manage Services
            </a>
          </div>

          <button
            onClick={() => setIsOpen(!isOpen)}
            className="md:hidden p-2"
            aria-label="Toggle menu"
          >
            {isOpen ? <X size={24} /> : <Menu size={24} />}
          </button>
        </div>

        {isOpen && (
          <div className="md:hidden py-4 border-t">
            <div className="flex flex-col space-y-4">
              {navLinks.map((link) => (
                <Link
                  key={link.href}
                  href={link.href}
                  className="text-gray-600 hover:text-primary transition-colors"
                  onClick={() => setIsOpen(false)}
                >
                  {link.label}
                </Link>
              ))}
              <Link
                href="/customer-portal"
                className="bg-primary text-white px-4 py-2 rounded hover:bg-primary/90 transition-colors text-center"
              >
                Customer Portal
              </Link>
              <a
                href="https://manager-web.ginilog.com/"
                target="_blank"
                rel="noopener noreferrer"
                className="bg-secondary text-white px-4 py-2 rounded hover:bg-secondary/90 transition-colors text-center"
              >
                Manage Services
              </a>
            </div>
          </div>
        )}
      </div>
    </nav>
  );
}
