import { apiRequest } from "@/lib/api/api-client";
import type {
  AdminDashboardResponse,
  AuditLogsResponse,
  ListingStatsResponse,
  ModelCategoriesResponse,
  ModelPerformanceCategoryResponse,
  ModelPerformanceListingsResponse,
  ModelPerformanceOverviewResponse,
  ModelStatsResponse,
  PredictionStatsResponse,
  UserStatsResponse,
} from "@/types/admin";

export const adminApi = {
  getDashboard(accessToken: string, signal?: AbortSignal): Promise<AdminDashboardResponse> {
    return apiRequest<AdminDashboardResponse>("/api/admin/dashboard", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getUserStats(accessToken: string, signal?: AbortSignal): Promise<UserStatsResponse> {
    return apiRequest<UserStatsResponse>("/api/admin/users/stats", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getListingStats(accessToken: string, signal?: AbortSignal): Promise<ListingStatsResponse> {
    return apiRequest<ListingStatsResponse>("/api/admin/listings/stats", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getPredictionStats(
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<PredictionStatsResponse> {
    return apiRequest<PredictionStatsResponse>("/api/admin/predictions/stats", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getModelStats(accessToken: string, signal?: AbortSignal): Promise<ModelStatsResponse> {
    return apiRequest<ModelStatsResponse>("/api/admin/models/stats", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getAuditLogs(accessToken: string, signal?: AbortSignal): Promise<AuditLogsResponse> {
    return apiRequest<AuditLogsResponse>("/api/admin/audit-logs", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  getModelPerformanceOverview(
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ModelPerformanceOverviewResponse> {
    return apiRequest<ModelPerformanceOverviewResponse>(
      "/api/admin/model-performance/overview",
      {
        method: "GET",
        accessToken,
        signal,
      },
    );
  },

  getModelPerformanceByCategory(
    category: string,
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ModelPerformanceCategoryResponse> {
    return apiRequest<ModelPerformanceCategoryResponse>(
      `/api/admin/model-performance/${category}`,
      {
        method: "GET",
        accessToken,
        signal,
      },
    );
  },

  getModelPerformanceListings(
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ModelPerformanceListingsResponse> {
    return apiRequest<ModelPerformanceListingsResponse>(
      "/api/admin/model-performance/listings",
      {
        method: "GET",
        accessToken,
        signal,
      },
    );
  },

  getModelCategories(
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ModelCategoriesResponse> {
    return apiRequest<ModelCategoriesResponse>("/api/model/categories", {
      method: "GET",
      accessToken,
      signal,
    });
  },
};