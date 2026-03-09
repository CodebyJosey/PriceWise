export interface AdminMetricItemResponse {
  name: string;
  value: number;
}

export interface AdminRecentAuditLogResponse {
  id: string;
  userId: string | null;
  action: string;
  entityType: string;
  entityId: string | null;
  details: string | null;
  createdAtUtc: string;
}

export interface AdminDashboardResponse {
  totalUsers: number;
  totalListings: number;
  publishedListings: number;
  soldListings: number;
  totalPredictions: number;
  totalTrainingRuns: number;
  totalAuditLogs: number;
  listingsLast24Hours: number;
  predictionsLast24Hours: number;
  listingsByCategory: AdminMetricItemResponse[];
  predictionsByCategory: AdminMetricItemResponse[];
  trainingRunsByCategory: AdminMetricItemResponse[];
  recentAuditLogs: AdminRecentAuditLogResponse[];
}

export interface UserStatsResponse {
  totalUsers: number;
  activeUsers: number;
  admins: number;
  sellers: number;
  buyers: number;
}

export interface ListingStatsCategoryItem {
  category: string;
  count: number;
}

export interface ListingStatsResponse {
  total: number;
  draft: number;
  predicted: number;
  published: number;
  sold: number;
  archived: number;
  byCategory: ListingStatsCategoryItem[];
}

export interface PredictionStatsCategoryItem {
  category: string;
  count: number;
}

export interface PredictionStatsModelVersionItem {
  category: string;
  modelVersion: number;
  count: number;
}

export interface PredictionStatsResponse {
  total: number;
  byCategory: PredictionStatsCategoryItem[];
  byModelVersion: PredictionStatsModelVersionItem[];
}

export interface ModelStatsCategoryItem {
  category: string;
  count: number;
  latestVersion: number;
}

export interface ModelStatsLatestRunItem {
  id: string;
  categoryKey: string;
  version: number;
  rSquared: number;
  rootMeanSquaredError: number;
  meanAbsoluteError: number;
  meanSquaredError: number;
  createdAtUtc: string;
}

export interface ModelStatsResponse {
  totalTrainingRuns: number;
  byCategory: ModelStatsCategoryItem[];
  latestRuns: ModelStatsLatestRunItem[];
}

export interface AuditLogsResponse {
  count: number;
  logs: AdminRecentAuditLogResponse[];
}

export interface ModelPerformanceOverviewMetric {
  category: string;
  totalListings: number;
  meanAbsoluteError: number;
  meanAbsolutePercentageError: number;
  medianAbsoluteError: number;
}

export interface ModelPerformanceOverviewResponse {
  summary: {
    totalCategories: number;
    totalListings: number;
    weightedMeanAbsoluteError: number;
    weightedMeanAbsolutePercentageError: number;
  };
  categories: ModelPerformanceOverviewMetric[];
}

export interface ModelPerformanceCategoryListingItem {
  listingId: string;
  title: string;
  actualPrice: number;
  predictedPrice: number;
  absoluteError: number;
  absolutePercentageError: number;
  soldAtUtc: string;
}

export interface ModelPerformanceCategoryResponse {
  category: string;
  totalListings: number;
  meanAbsoluteError: number;
  meanAbsolutePercentageError: number;
  medianAbsoluteError: number;
  listings: ModelPerformanceCategoryListingItem[];
}

export interface ModelPerformanceRecentListingItem {
  listingId: string;
  category: string;
  title: string;
  actualPrice: number;
  predictedPrice: number;
  absoluteError: number;
  absolutePercentageError: number;
  soldAtUtc: string;
}

export type ModelPerformanceListingsResponse =
  ModelPerformanceRecentListingItem[];

export interface ModelCategoryItem {
  key: string;
  datasetFileName: string;
  modelDirectoryPath: string;
  metadataDirectoryPath: string;
  featuresType: string;
  trainingRowType: string;
  predictionType: string;
}

export interface ModelCategoriesResponse {
  count: number;
  categories: ModelCategoryItem[];
}