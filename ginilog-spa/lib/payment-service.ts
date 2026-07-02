// lib/payment-service.ts

import { 
  initializePaystackPayment, 
  verifyPaystackPayment,
  initializeFlutterwavePayment,
  verifyFlutterwavePayment,
  PaystackInitializeRequest,
  FlutterwaveInitializeRequest,
  getStoredUser
} from './api';

export interface PaymentResult {
  success: boolean;
  message: string;
  reference?: string;
  data?: any;
}

export class PaymentService {
  private static instance: PaymentService;
  private paystackScriptLoaded = false;
  private flutterwaveScriptLoaded = false;

  private constructor() {}

  public static getInstance(): PaymentService {
    if (!PaymentService.instance) {
      PaymentService.instance = new PaymentService();
    }
    return PaymentService.instance;
  }

  // Load Paystack script
  public async loadPaystackScript(): Promise<void> {
    if (this.paystackScriptLoaded) return;
    
    return new Promise((resolve, reject) => {
      if (document.querySelector('#paystack-script')) {
        this.paystackScriptLoaded = true;
        resolve();
        return;
      }
      
      const script = document.createElement('script');
      script.id = 'paystack-script';
      script.src = 'https://js.paystack.co/v1/inline.js';
      script.onload = () => {
        this.paystackScriptLoaded = true;
        resolve();
      };
      script.onerror = () => {
        reject(new Error('Failed to load Paystack script'));
      };
      document.body.appendChild(script);
    });
  }

  // Load Flutterwave script
  public async loadFlutterwaveScript(): Promise<void> {
    if (this.flutterwaveScriptLoaded) return;
    
    return new Promise((resolve, reject) => {
      if (document.querySelector('#flutterwave-script')) {
        this.flutterwaveScriptLoaded = true;
        resolve();
        return;
      }
      
      const script = document.createElement('script');
      script.id = 'flutterwave-script';
      script.src = 'https://checkout.flutterwave.com/v3.js';
      script.onload = () => {
        this.flutterwaveScriptLoaded = true;
        resolve();
      };
      script.onerror = () => {
        reject(new Error('Failed to load Flutterwave script'));
      };
      document.body.appendChild(script);
    });
  }

  // Process Paystack payment
  public async processPaystackPayment(
    amount: number,
    email: string,
    orderId?: string,
    metadata?: Record<string, any>
  ): Promise<PaymentResult> {
    try {
      // Load Paystack script
      await this.loadPaystackScript();
      
      // Get user from localStorage
      const user = getStoredUser();
      if (!user) {
        throw new Error('User not authenticated');
      }

      // Initialize payment on backend
      const initData: PaystackInitializeRequest = {
        amount,
        email,
        orderId,
        metadata: {
          ...metadata,
          userId: user.userId,
          userType: user.userType,
          timestamp: new Date().toISOString()
        }
      };
      
      const response = await initializePaystackPayment(initData);
      
      if (!response.status && !response.reference) {
        throw new Error(response.message || 'Failed to initialize payment');
      }

      const reference = response.data?.reference || response.reference;
      
      // Initialize Paystack inline checkout
      const paystack = new (window as any).PaystackPop();
      
      return new Promise((resolve, reject) => {
        paystack.newTransaction({
          key: process.env.NEXT_PUBLIC_PAYSTACK_PUBLIC_KEY,
          email: email,
          amount: amount * 100, // Paystack expects amount in kobo
          ref: reference,
          metadata: {
            ...metadata,
            orderId: orderId || '',
            userId: user.userId
          },
          callback: async (response: any) => {
            try {
              // Verify payment
              const verification = await verifyPaystackPayment(response.reference);
              
              if (verification.status && verification.data.status === 'success') {
                resolve({
                  success: true,
                  message: 'Payment successful',
                  reference: response.reference,
                  data: verification.data
                });
              } else {
                reject(new Error('Payment verification failed'));
              }
            } catch (error) {
              reject(error);
            }
          },
          onClose: () => {
            reject(new Error('Payment was cancelled'));
          }
        });
      });
    } catch (error) {
      console.error('Paystack payment error:', error);
      throw error;
    }
  }

  // Process Flutterwave payment
  public async processFlutterwavePayment(
    amount: number,
    email: string,
    fullName: string,
    phoneNumber?: string,
    metadata?: Record<string, any>
  ): Promise<PaymentResult> {
    try {
      // Load Flutterwave script
      await this.loadFlutterwaveScript();
      
      // Get user from localStorage
      const user = getStoredUser();
      if (!user) {
        throw new Error('User not authenticated');
      }

      // Generate unique transaction reference
      const tx_ref = `TX-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
      
      // Initialize payment on backend
      const initData: FlutterwaveInitializeRequest = {
        amount,
        email,
        fullName,
        phoneNumber: phoneNumber || user.phoneNo || '',
        tx_ref,
        metadata: {
          ...metadata,
          userId: user.userId,
          userType: user.userType,
          timestamp: new Date().toISOString()
        }
      };
      
      const response = await initializeFlutterwavePayment(initData);
      
      if (response.status !== 'success' && !response.tx_ref) {
        throw new Error(response.message || 'Failed to initialize payment');
      }

      const txRef = response.data?.tx_ref || response.tx_ref || tx_ref;
      
      // Initialize Flutterwave checkout
      const FlutterwaveCheckout = (window as any).FlutterwaveCheckout;
      
      return new Promise((resolve, reject) => {
        FlutterwaveCheckout({
          public_key: process.env.NEXT_PUBLIC_FLUTTERWAVE_PUBLIC_KEY,
          tx_ref: txRef,
          amount: amount,
          currency: 'NGN',
          payment_options: 'card, banktransfer, ussd, qr, mobilemoneyghana, mobilemoneyuganda',
          customer: {
            email: email,
            phone_number: phoneNumber || user.phoneNo || '',
            name: fullName,
          },
          customizations: {
            title: 'Ginilog Booking',
            description: 'Accommodation Booking Payment',
            logo: 'https://www.ginilog.com/logo.png',
          },
          meta: {
            ...metadata,
            userId: user.userId
          },
          callback: async (payment: any) => {
            try {
              // Verify payment
              const verification = await verifyFlutterwavePayment(payment.transaction_id || payment.tx_ref);
              
              if (verification.status === 'success') {
                resolve({
                  success: true,
                  message: 'Payment successful',
                  reference: payment.transaction_id || payment.tx_ref,
                  data: verification
                });
              } else {
                reject(new Error('Payment verification failed'));
              }
            } catch (error) {
              reject(error);
            }
          },
          onclose: () => {
            reject(new Error('Payment was cancelled'));
          }
        });
      });
    } catch (error) {
      console.error('Flutterwave payment error:', error);
      throw error;
    }
  }

  // Process payment with selected method
  public async processPayment(
    method: 'paystack' | 'flutterwave',
    amount: number,
    email: string,
    fullName: string,
    phoneNumber?: string,
    metadata?: Record<string, any>
  ): Promise<PaymentResult> {
    if (method === 'paystack') {
      return this.processPaystackPayment(amount, email, metadata?.orderId, metadata);
    } else if (method === 'flutterwave') {
      return this.processFlutterwavePayment(amount, email, fullName, phoneNumber, metadata);
    } else {
      throw new Error('Unsupported payment method');
    }
  }
}

export default PaymentService;