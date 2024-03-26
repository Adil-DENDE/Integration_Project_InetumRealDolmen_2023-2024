﻿using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;
using ModelLibrary.Dto;

namespace RealDolmenAPI.Controllers
{
    public class OccupationHistoryController
    {
        public static void Map(WebApplication app)
        {
            var occupationHistoryGroup = app.MapGroup("/occupationHistory");

            // POST: Voeg een occupation history record toe
            occupationHistoryGroup.MapPost("/add", async (OccupationHistoryDto dto, AppDbContext db) =>
            {
                // Vind de occupation met ID
                var occupation = await db.Occupation.FirstOrDefaultAsync(o => o.Id == dto.OccupationId);
                if (occupation == null)
                {
                    return Results.NotFound("Occupation niet gevonden.");
                }

                // Vind de bench op basis van ID
                var bench = await db.Bench.FindAsync(dto.BenchId);
                if (bench == null)
                {
                    return Results.NotFound("Bench niet gevonden.");
                }

                // Maak en voeg nieuwe OccupationHistory record toe
                var occupationHistory = new OccupationHistory
                {
                    Bench_id = dto.BenchId,
                    Occupation_id = dto.OccupationId,
                    Start_occupationdate = dto.StartDate,
                    End_occupationdate = dto.EndDate
                };

                db.OccupationHistory.Add(occupationHistory);
                await db.SaveChangesAsync();

                return Results.Ok("Occupation history succesvol toegevoegd.");
            });

            // GET: Haal alle occupation histories op voor een specifieke benchId waar End_occupationdate null is
            occupationHistoryGroup.MapGet("/active/{benchId:int}", async (int benchId, AppDbContext db) =>
            {
                var activeOccupationHistories = await db.OccupationHistory
                    .Where(oh => oh.Bench_id == benchId && oh.End_occupationdate == null)
                    .ToListAsync();

                if (!activeOccupationHistories.Any())
                {
                    return Results.NotFound($"Geen actieve occupation histories gevonden voor benchId: {benchId}.");
                }

                return Results.Ok(activeOccupationHistories);
            });

            // PUT: Update de end date van de laatste OccupationHistory record voor een gegeven benchId
            occupationHistoryGroup.MapPut("/end/{benchId:int}", async (int benchId, OccupationHistoryDto dto, AppDbContext db) =>
            {
                var lastOccupationHistory = await db.OccupationHistory
                    .Where(oh => oh.Bench_id == benchId && oh.End_occupationdate == null)
                    .OrderByDescending(oh => oh.Start_occupationdate)
                    .FirstOrDefaultAsync();

                if (lastOccupationHistory == null)
                {
                    return Results.NotFound($"Geen actieve occupation history gevonden voor benchId: {benchId}.");
                }

                lastOccupationHistory.End_occupationdate = dto.EndDate;
                await db.SaveChangesAsync();

                return Results.Ok("De activiteit is succesvol beëindigd.");
            });

            // PUT: Update de end date van alle actieve OccupationHistory records voor een benchId
            occupationHistoryGroup.MapPut("/endAll/{benchId:int}", async (int benchId, DateTime endDate, AppDbContext db) =>
            {
                var activeOccupationHistories = await db.OccupationHistory
                    .Where(oh => oh.Bench_id == benchId && oh.End_occupationdate == null)
                    .ToListAsync();

                if (!activeOccupationHistories.Any())
                {
                    return Results.NotFound($"Geen actieve occupation histories gevonden voor benchId: {benchId}.");
                }

                foreach (var history in activeOccupationHistories)
                {
                    history.End_occupationdate = endDate;
                }

                await db.SaveChangesAsync();

                return Results.Ok($"Alle actieve occupation histories voor benchId: {benchId} zijn succesvol geëindigd.");
            });


        }
    }
}