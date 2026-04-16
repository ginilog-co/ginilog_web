"use client";

import { useState } from "react";
import Link from "next/link";
import { Truck, Hotel, Globe, Shield, Clock, Headphones, Package, CheckCircle, CreditCard, Key, Calendar, User, Send } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export default function LandingPage() {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    message: "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Form submitted:", formData);
  };

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative h-[600px] bg-gray-900 overflow-hidden">
        <div 
          className="absolute inset-0 bg-cover bg-center"
          style={{ backgroundImage: "url('/carousel-1.jpg')" }}
        />
        <div className="absolute inset-0 bg-gradient-to-r from-black/70 to-black/30" />
        <div className="container mx-auto px-4 h-full flex items-center relative z-10">
          <div className="max-w-2xl text-white">
            <h5 className="text-sm font-semibold tracking-wider uppercase mb-4">Welcome to GINILOG</h5>
            <h1 className="text-4xl md:text-5xl font-bold mb-6">
              A One Stop Shop for All Your Logistics and Bookings
            </h1>
            <p className="text-lg text-gray-200 mb-8">
              Efficient and tailored to your unique needs. Founded by a team of innovative minds 
              who recognized the growing complexity in coordinating logistics and accommodation.
            </p>
            <Link href="/customer-portal/login">
              <Button size="lg" className="bg-primary hover:bg-primary/90">
                Get Started
              </Button>
            </Link>
          </div>
        </div>
      </section>

      {/* About Section */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div className="relative h-[400px] rounded-lg overflow-hidden shadow-xl">
              <img 
                src="/about.jpg" 
                alt="Ginilog Platform" 
                className="w-full h-full object-cover"
              />
            </div>
            <div>
              <h6 className="text-sm font-semibold tracking-wider uppercase text-primary mb-2">About Us</h6>
              <h2 className="text-3xl font-bold mb-6">Logistics and Accommodation Solutions</h2>
              <p className="text-gray-600 mb-6">
                At GINILOG, accommodation booking is reimagined as a hassle-free and delightful experience.
                We believe that finding the perfect place to stay should enhance the joy of any journey. By 
                integrating user-friendly tools, personalised recommendations, and real-time support, we
                ensure that every step of the booking process is tailored to meet your needs.
              </p>
              <p className="text-gray-600 mb-6">
                Want to manage your accommodation/logistics? Sign up as a manager to manage your 
                accommodation/logistics services with us for free.
              </p>
              <a 
                href="https://manager-web.ginilog.com/" 
                target="_blank" 
                rel="noopener noreferrer"
                className="inline-flex items-center justify-center rounded-md bg-primary px-6 py-3 text-base font-medium text-white hover:bg-primary/90 transition-colors"
              >
                Manage Services
              </a>
            </div>
          </div>
        </div>
      </section>

      {/* Services Section */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h6 className="text-sm font-semibold tracking-wider uppercase text-primary mb-2">Our Services</h6>
            <h2 className="text-3xl font-bold">Explore Our Services</h2>
          </div>
          <div className="grid md:grid-cols-3 gap-8">
            {/* Logistics */}
            <div className="bg-white p-6 rounded-lg shadow-sm border hover:shadow-md transition-shadow">
              <div className="h-48 rounded-lg mb-4 overflow-hidden">
                <img src="/service-1.jpg" alt="Logistics" className="w-full h-full object-cover" />
              </div>
              <h4 className="text-xl font-semibold mb-3">Logistics Solutions</h4>
              <p className="text-gray-600 mb-4">
                GINILOG Logistics, where excellence in delivering your orders is just the beginning.
                We are dedicated to providing exceptional logistics services and knowledge to make informed decisions.
              </p>
              <ul className="text-sm text-gray-600 space-y-1">
                <li>• Warehouse and distribution</li>
                <li>• Supply chain management and optimization</li>
                <li>• Route connections and optimization</li>
              </ul>
            </div>

            {/* Delivery */}
            <div className="bg-white p-6 rounded-lg shadow-sm border hover:shadow-md transition-shadow">
              <div className="h-48 rounded-lg mb-4 overflow-hidden">
                <img src="/service-2.jpg" alt="Delivery" className="w-full h-full object-cover" />
              </div>
              <h4 className="text-xl font-semibold mb-3">Delivery Solutions</h4>
              <p className="text-gray-600 mb-4">
                GINILOG Logistics, where excellence in delivering your orders is just the beginning.
                We are dedicated to providing exceptional logistics services and knowledge to make informed decisions.
              </p>
              <ul className="text-sm text-gray-600 space-y-1">
                <li>• Your Product is safe with us</li>
                <li>• We serve industries</li>
                <li>• Exclusive and luxurious stays</li>
              </ul>
            </div>

            {/* Bookings */}
            <div className="bg-white p-6 rounded-lg shadow-sm border hover:shadow-md transition-shadow">
              <div className="h-48 rounded-lg mb-4 overflow-hidden">
                <img src="/service-3.jpg" alt="Bookings" className="w-full h-full object-cover" />
              </div>
              <h4 className="text-xl font-semibold mb-3">Bookings Solution</h4>
              <p className="text-gray-600 mb-4">
                GINILOG Logistics, where excellence in delivering your orders is just the beginning.
                We are dedicated to providing exceptional logistics services and knowledge to make informed decisions.
              </p>
              <ul className="text-sm text-gray-600 space-y-1">
                <li>• Hotel and resort bookings</li>
                <li>• Serviced apartments and vacation rentals</li>
                <li>• Exclusive and luxurious stays</li>
              </ul>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div>
              <h6 className="text-sm font-semibold tracking-wider uppercase text-primary mb-2">Our Features</h6>
              <h2 className="text-3xl font-bold mb-6">Experience Seamless Logistics Solutions and Community Engagement</h2>
              <div className="space-y-6">
                <div className="flex items-start">
                  <Globe className="h-8 w-8 text-primary flex-shrink-0 mt-1" />
                  <div className="ml-4">
                    <h5 className="font-semibold mb-1">Inclusive Community</h5>
                    <p className="text-gray-600">Everyone is welcome and has a role to play in our community.</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <Shield className="h-8 w-8 text-primary flex-shrink-0 mt-1" />
                  <div className="ml-4">
                    <h5 className="font-semibold mb-1">Seamless Trip Planning</h5>
                    <p className="text-gray-600">We offer a stress-free and enjoyable trip planning experience.</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <Clock className="h-8 w-8 text-primary flex-shrink-0 mt-1" />
                  <div className="ml-4">
                    <h5 className="font-semibold mb-1">Knowledge and Solutions</h5>
                    <p className="text-gray-600">Access the information you need to make informed logistics decisions.</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <Headphones className="h-8 w-8 text-primary flex-shrink-0 mt-1" />
                  <div className="ml-4">
                    <h5 className="font-semibold mb-1">Community Growth</h5>
                    <p className="text-gray-600">Join a community that grows together and supports each other&apos;s success.</p>
                  </div>
                </div>
              </div>
            </div>
            <div className="relative h-[400px] rounded-lg overflow-hidden shadow-xl">
              <img 
                src="/communityEngage.png" 
                alt="Community Engagement" 
                className="w-full h-full object-cover"
              />
            </div>
          </div>
        </div>
      </section>

      {/* How It Works - Logistics */}
      <section className="py-20">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold mb-2">How Logistics Order Works</h2>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
            {[
              { icon: User, step: "ONE", title: "Courier Requested" },
              { icon: CheckCircle, step: "TWO", title: "Package is Ready" },
              { icon: Truck, step: "THREE", title: "Package in Transit" },
              { icon: Package, step: "FOUR", title: "Package in Transit" },
              { icon: Key, step: "FIVE", title: "Package Delivered" },
            ].map((item, index) => (
              <div key={index} className="bg-white p-4 rounded-lg shadow-sm border text-center">
                <item.icon className="h-8 w-8 text-primary mx-auto mb-2" />
                <div className="text-xs font-semibold text-primary">STEP {item.step}</div>
                <div className="text-sm font-medium">{item.title}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* How It Works - Accommodation */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold mb-2">How Accommodation Booking Works</h2>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
            {[
              { icon: User, step: "ONE", title: "Search for Accommodation" },
              { icon: Calendar, step: "TWO", title: "Select Dates & Room Type" },
              { icon: User, step: "THREE", title: "Provide Guest Details" },
              { icon: CreditCard, step: "FOUR", title: "Make Payment" },
              { icon: Key, step: "FIVE", title: "Check-In & Enjoy Stay" },
            ].map((item, index) => (
              <div key={index} className="bg-white p-4 rounded-lg shadow-sm border text-center">
                <item.icon className="h-8 w-8 text-primary mx-auto mb-2" />
                <div className="text-xs font-semibold text-primary">STEP {item.step}</div>
                <div className="text-sm font-medium">{item.title}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Contact Section */}
      <section id="contact" className="py-20">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h6 className="text-sm font-semibold tracking-wider uppercase text-primary mb-2">Contact Us</h6>
            <h2 className="text-3xl font-bold">Get In Touch</h2>
          </div>
          <div className="grid md:grid-cols-2 gap-12 max-w-6xl mx-auto">
            {/* Contact Form */}
            <div className="bg-white p-8 rounded-lg shadow-sm border">
              <h3 className="text-2xl font-bold mb-6">Send Us a Message</h3>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <Label htmlFor="name">Name</Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFormData({ ...formData, name: e.target.value })}
                    placeholder="Your name"
                  />
                </div>
                <div>
                  <Label htmlFor="email">Email</Label>
                  <Input
                    id="email"
                    type="email"
                    value={formData.email}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFormData({ ...formData, email: e.target.value })}
                    placeholder="Your email"
                  />
                </div>
                <div>
                  <Label htmlFor="phone">Phone</Label>
                  <Input
                    id="phone"
                    value={formData.phone}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => setFormData({ ...formData, phone: e.target.value })}
                    placeholder="Your phone number"
                  />
                </div>
                <div>
                  <Label htmlFor="message">Message</Label>
                  <textarea
                    id="message"
                    rows={4}
                    className="w-full px-3 py-2 border rounded-md"
                    value={formData.message}
                    onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => setFormData({ ...formData, message: e.target.value })}
                    placeholder="Your message"
                  />
                </div>
                <Button type="submit" className="w-full">
                  <Send className="h-4 w-4 mr-2" />
                  Send Message
                </Button>
              </form>
            </div>

            {/* Map */}
            <div className="h-full min-h-[400px] bg-gray-200 rounded-lg overflow-hidden">
              <iframe
                className="w-full h-full rounded-lg"
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3971.0813707863424!2d7.49857891426392!3d6.440179725233897!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x1044d397e06b4f09%3A0x8698c4c3798d9a4f!2sIndependence%20Layout%2C%20Enugu!5e0!3m2!1sen!2sng!4v1717667389012!5m2!1sen!2sng"
                allowFullScreen
                loading="lazy"
              />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
