namespace BatchRunner.DataModel;

using BatchRunner.Extensions;
using BatchRunner.Hardware;
using Microsoft.Data.Sqlite;
using System;
using System.Threading;

public enum ChannelState {
    Off = 0,
    On = 1,
}


public record Step(
    string Channel,
    ChannelState State,
    bool ApplyAfter,
    int? DelayBefore,
    int? DelayAfter
){
    public Step(string Channel, ChannelState State)
        : this(Channel, State, false, null, null){}

    public Step(string Channel, ChannelState State, bool ApplyAfter)
        : this(Channel, State, ApplyAfter, null, null){}

    public Step(string Channel, ChannelState State, int? DelayBefore = null, int? DelayAfter = null)
        : this(Channel, State, false, DelayBefore, DelayAfter){}
}


class BatchRecipe {
    const string sqlDropTable = "drop table if exists BatchRecipe";
    const string sqlCreateTable = @"
        create table BatchRecipe (
            id integer primary key,
            recipe text not null,
            step int not null,
            channel text not null,
            state int not null check(state in (0, 1)),
            -- Delays are in milliseconds
            delayBefore int,
            delayAfter int,
            -- Set when this step should cause the channel state to get sent to hardware instead
            -- of waiting for the next step with a delay
            applyAfter int not null check(applyAfter in (0, 1))
        ) strict";
    const string sqlInsert = @"
        insert into BatchRecipe (
            recipe, step, channel, state, delayBefore, delayAfter, applyAfter
        ) values (
            @recipe, @step, @channel, @state, @delayBefore, @delayAfter, @applyAfter
        )";
    const string sqlDelete = "delete from BatchRecipe where recipe = @recipe";
    const string sqlRecipeSteps = "select * from BatchRecipe where recipe = @recipe";

    SqliteConnection connection;

    public BatchRecipe() {
        connection = new SqliteConnection("Data Source=ep5BAS.db");
        connection.Open();
    }

    public void CreateTable() {
        var command = connection.CreateCommand();

        command.CommandText = sqlDropTable;
        command.ExecuteNonQuery();

        command.CommandText = sqlCreateTable;
        command.ExecuteNonQuery();
    }

    public void CreateRecipe(string recipeName, Step[] steps) {
        var command = connection.CreateCommand();

        command.CommandText = sqlDelete;
        command.Parameters.AddWithValue("@recipe", recipeName);
        command.ExecuteNonQuery();

        command.CommandText = sqlInsert;
        command.Parameters.Add(new SqliteParameter("@step", SqliteType.Integer));
        command.Parameters.Add(new SqliteParameter("@channel", SqliteType.Text));
        command.Parameters.Add(new SqliteParameter("@state", SqliteType.Integer));
        command.Parameters.Add(new SqliteParameter("@delayBefore", SqliteType.Integer));
        command.Parameters.Add(new SqliteParameter("@delayAfter", SqliteType.Integer));
        command.Parameters.Add(new SqliteParameter("@applyAfter", SqliteType.Integer));


        foreach ((var i, var step) in steps.Enumerate()) {
            command.Parameters["@step"].Value = i;
            command.Parameters["@channel"].Value = step.Channel;
            command.Parameters["@state"].Value = step.State;
            command.Parameters["@delayBefore"].Value = step.DelayBefore ?? (object)DBNull.Value;
            command.Parameters["@delayAfter"].Value = step.DelayAfter ?? (object)DBNull.Value;
            command.Parameters["@applyAfter"].Value = step.ApplyAfter;
            command.ExecuteNonQuery();
        }
    }

    private IEnumerable<Step> recipeSteps(string recipeName)
    {
        using (SqliteCommand command = new SqliteCommand(sqlRecipeSteps, connection))
        {
            command.Parameters.AddWithValue("@recipe", recipeName);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string channel = reader.GetString("channel");
                    ChannelState state = reader.GetEnum<ChannelState>("state");
                    bool applyAfter = reader.GetBoolean("applyAfter");
                    int? delayBefore = reader.GetNInt32("delayBefore");
                    int? delayAfter = reader.GetNInt32("delayAfter");

                    yield return new Step(channel, state, applyAfter, delayBefore, delayAfter);
                }
            }
        }
    }

    public void Run(string recipeName, iHardware hardware) {

        foreach (var step in recipeSteps(recipeName)) {
            // Console.WriteLine($"Running step: {step}");
            // TODO: handle step.delayBefore
            hardware.SetChannel(step.Channel, (int)step.State);
            if (step.ApplyAfter || step.DelayAfter != null) {
                hardware.Apply();
            }
            if (step.DelayAfter != null) {
                Console.WriteLine($"Delaying (ms): {step.DelayAfter}");
                Thread.Sleep(step.DelayAfter.Value);
            }
        }

    }
}
