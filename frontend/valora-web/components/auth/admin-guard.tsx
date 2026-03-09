"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { getAccessToken } from "@/lib/auth/token-storage";
import { useCurrentUser } from "@/hooks/use-current-user";

interface AdminGuardProps {
  children: React.ReactNode;
}

export function AdminGuard({ children }: AdminGuardProps): React.JSX.Element {
  const router = useRouter();
  const pathname = usePathname();
  const token = getAccessToken();
  const { user, isAuthenticated, isLoading } = useCurrentUser();

  useEffect(() => {
    if (!token) {
      const returnUrl = encodeURIComponent(pathname ?? "/admin");
      router.replace(`/login?returnUrl=${returnUrl}`);
    }
  }, [pathname, router, token]);

  useEffect(() => {
    if (!isLoading && token && !isAuthenticated) {
      const returnUrl = encodeURIComponent(pathname ?? "/admin");
      router.replace(`/login?returnUrl=${returnUrl}`);
      return;
    }

    if (!isLoading && isAuthenticated && user && !user.roles.includes("Admin")) {
      router.replace("/dashboard");
    }
  }, [isAuthenticated, isLoading, pathname, router, token, user]);

  if (!token || isLoading || !isAuthenticated || !user || !user.roles.includes("Admin")) {
    return (
      <main className="mx-auto flex min-h-[60vh] w-full max-w-7xl items-center justify-center px-6 py-16 lg:px-8">
        <div className="soft-panel rounded-[2rem] p-8 text-center">
          <h1 className="text-2xl font-black text-emerald-950">
            Adminrechten controleren...
          </h1>
          <p className="mt-3 text-sm leading-7 text-emerald-950/65">
            We checken of je toegang hebt tot het admin dashboard.
          </p>
        </div>
      </main>
    );
  }

  return <>{children}</>;
}